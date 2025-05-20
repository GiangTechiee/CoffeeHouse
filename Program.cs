using CoffeeHouse;
using Microsoft.EntityFrameworkCore;
using CoffeeHouse.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Đăng ký IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Cấu hình DbContext với PostgreSQL
builder.Services.AddDbContext<CoffeeHouseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CoffeeHouseDb")));

builder.Services.AddScoped<INhomSpRepository, NhomSpRepository>();
builder.Services.AddScoped<ShoppingCartSummaryViewComponent>();

// Cấu hình cache phân tán và Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    // Bạn có thể thiết lập các tùy chọn cho Session nếu cần
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Chỉ sử dụng HTTPS redirection trong môi trường development

app.UseHttpsRedirection();


app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();
app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
