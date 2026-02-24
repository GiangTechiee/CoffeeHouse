using CoffeeHouse.Application;
using CoffeeHouse.Infrastructure;
using CoffeeHouse.Middleware;
using Serilog;
using CoffeeHouse.Web.ViewComponents;
using CoffeeHouse.Domain.Identity;
using CoffeeHouse.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using CoffeeHouse.Web.Configuration;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Microsoft.FeatureManagement;
using Asp.Versioning;

// Configure Serilog from appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .AddEnvironmentVariables()  // Override with environment variables
        .Build())
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog as the logging provider
    builder.Host.UseSerilog();

    // Add general services
    builder.Services.AddHttpContextAccessor();

    // Add layers
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // Configure Identity
    builder.Services.AddIdentity<AppUser, AppRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<CoffeeHouseContext>()
    .AddDefaultTokenProviders();

    // Configure JWT Authentication
    // Configure JWT Authentication
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"];

    if (string.IsNullOrEmpty(secretKey) || secretKey == "CHANGE_ME_MIN_32_CHARACTERS")
    {
        if (builder.Environment.IsProduction())
        {
            throw new InvalidOperationException("Available JWT SecretKey is insecure or missing in Production environment.");
        }
        // Dev safe fallback
        secretKey = "CHANGE_ME_MIN_32_CHARACTERS_FOR_DEV_ONLY_12345"; 
    }
    
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

    // Add Presentation layer services
    builder.Services.AddControllersWithViews();
    
    // Configure CORS for React frontend
    var corsOrigins = builder.Configuration["CorsSettings:AllowedOrigins"]?
        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        ?? new[] { "http://localhost:3000" };
    
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("ReactApp", policy =>
        {
            policy.WithOrigins(corsOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();              // Cho phép gửi cookies/session
        });
    });
    
    // Configure Session
    // Use Redis instead of In-Memory Cache
    // builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(20);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    // Register ViewComponents
    builder.Services.AddScoped<ShoppingCartSummaryViewComponent>();
    builder.Services.AddScoped<NhomSpMenuViewComponent>();

    // Configure Rate Limiting
    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        options.AddPolicy("AuthPolicy", context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                }));

        options.AddPolicy("ApiPolicy", context =>
            RateLimitPartition.GetSlidingWindowLimiter(
                partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                factory: _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1),
                    SegmentsPerWindow = 4,
                    QueueLimit = 10
                }));
    });

    builder.Services.AddAntiforgery(options =>
    {
        options.HeaderName = "X-CSRF-TOKEN";
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

    builder.Services.AddResponseCaching(options =>
    {
        options.MaximumBodySize = 1024 * 1024; // 1MB
        options.UseCaseSensitivePaths = false;
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "CoffeeHouse API",
            Version = "v1",
            Description = "API for CoffeeHouse Management System"
        });

        // Add JWT Authentication to Swagger
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer 12345abcdef'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        // Include XML comments
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    });

    builder.Services.AddHealthChecks()
        .AddNpgSql(builder.Configuration.GetConnectionString("CoffeeHouseDb")!)
        .AddDbContextCheck<CoffeeHouseContext>();

    // Configure Options with validation
    builder.Services.AddOptions<JwtSettings>()
        .BindConfiguration(JwtSettings.SectionName)
        .ValidateDataAnnotations()
        .ValidateOnStart();

    builder.Services.AddOptions<CorsSettings>()
        .BindConfiguration(CorsSettings.SectionName)
        .ValidateDataAnnotations()
        .ValidateOnStart();

    // Configure OpenTelemetry
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource.AddService("CoffeeHouse.Web"))
        .WithTracing(tracing =>
        {
            tracing
                .AddAspNetCoreInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSource("MediatR")
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(builder.Configuration["OpenTelemetry:Endpoint"] ?? "http://localhost:4317");
                });
        })
        .WithMetrics(metrics =>
        {
            metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(builder.Configuration["OpenTelemetry:Endpoint"] ?? "http://localhost:4317");
                });
        });

    builder.Services.AddFeatureManagement();

    // Configure API Versioning
    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = ApiVersionReader.Combine(
            new QueryStringApiVersionReader("api-version"),
            new HeaderApiVersionReader("X-Api-Version")
        );
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "CoffeeHouse API v1");
            options.RoutePrefix = "api-docs";
        });
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
        app.UseHttpsRedirection();
    }

    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseStaticFiles();

    app.UseRouting();

    // CORS must be after UseRouting and before any other middleware that might block the request
    app.UseCors("ReactApp");

    app.UseResponseCaching();

    app.UseRateLimiter();

    app.UseAntiforgery();

    // Session must be before Authentication
    app.UseSession();
    
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseSerilogRequestLogging();

    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "text/html";
            var statusColor = report.Status == Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy ? "#2D6A4F" : "#BC4749";
            var statusIcon = report.Status == Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy ? "✅" : "⚠️";

            var rows = string.Join("", report.Entries.Select(e => $@"
                <tr>
                    <td style=""padding: 15px; border-bottom: 1px solid #eee;""><b>{e.Key}</b></td>
                    <td style=""padding: 15px; border-bottom: 1px solid #eee;"">
                        <span style=""padding: 4px 12px; border-radius: 20px; font-size: 0.8rem; background: {(e.Value.Status == Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy ? "#E8F5E9; color: #2D6A4F" : "#FFEBEE; color: #C62828")}"">
                            {(e.Value.Status == Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy ? "Đang hoạt động" : "Gặp sự cố")}
                        </span>
                    </td>
                    <td style=""padding: 15px; border-bottom: 1px solid #eee; color: #666; font-size: 0.9rem;"">{e.Value.Description ?? "Hệ thống hoạt động ổn định"}</td>
                    <td style=""padding: 15px; border-bottom: 1px solid #eee; text-align: right; color: #999;"">{e.Value.Duration.TotalMilliseconds:N0}ms</td>
                </tr>"));

            await context.Response.WriteAsync($@"
<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <title>Trạng thái Hệ thống - CoffeeHouse</title>
    <link href=""https://fonts.googleapis.com/css2?family=Outfit:wght@300;400;600&display=swap"" rel=""stylesheet"">
    <style>
        body {{ font-family: 'Outfit', sans-serif; background: #F8F9FA; color: #2C1810; margin: 0; padding: 40px; }}
        .card {{ background: white; max-width: 900px; margin: 0 auto; border-radius: 20px; box-shadow: 0 10px 30px rgba(0,0,0,0.05); overflow: hidden; }}
        .header {{ background: {statusColor}; color: white; padding: 40px; text-align: center; }}
        .content {{ padding: 40px; }}
        table {{ width: 100%; border-collapse: collapse; }}
        .tech-note {{ background: #F1F8E9; border-left: 4px solid #4CAF50; padding: 20px; margin-top: 30px; border-radius: 0 12px 12px 0; font-size: 0.9rem; line-height: 1.6; color: #33691E; }}
    </style>
</head>
<body>
    <div class=""card"">
        <div class=""header"">
            <div style=""font-size: 3rem; margin-bottom: 10px;"">{statusIcon}</div>
            <h1 style=""margin: 0; font-weight: 600;"">Kiểm tra Sức khỏe Hệ thống</h1>
            <p style=""opacity: 0.8;"">Trạng thái tổng quát: <b>{(report.Status == Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy ? "ỔN ĐỊNH" : "CẦN KIỂM TRA")}</b> &bull; Cập nhật lúc {DateTime.Now:HH:mm:ss}</p>
        </div>
        <div class=""content"">
            <table>
                <thead>
                    <tr style=""text-align: left; color: #999; text-transform: uppercase; font-size: 0.7rem; letter-spacing: 0.1em;"">
                        <th style=""padding: 15px;"">Thành phần</th>
                        <th style=""padding: 15px;"">Trạng thái</th>
                        <th style=""padding: 15px;"">Chi tiết</th>
                        <th style=""padding: 15px; text-align: right;"">Độ trễ</th>
                    </tr>
                </thead>
                <tbody>
                    {rows}
                </tbody>
            </table>

            <div class=""tech-note"">
                <b>📌 Đặc điểm kỹ thuật:</b><br/>
                Hệ thống áp dụng kiến trúc <b>Duy trì Tính sẵn sàng (High Availability)</b>. Các bộ kiểm tra (Health Checks) được cấu hình riêng biệt cho tầng dữ liệu (PostgreSQL) và tầng ứng dụng (Entity Framework Core). 
                Cơ chế này cho phép các hệ thống điều phối (Orchestrator) tự động phát hiện và xử lý sự cố mà không cần can thiệp thủ công.
            </div>
            
            <div style=""margin-top: 20px; text-align: center;"">
                <a href=""/"" style=""color: #999; text-decoration: none; font-size: 0.8rem;"">← Quay lại Bảng điều khiển</a>
            </div>
        </div>
    </div>
</body>
</html>");
        }
    });

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
