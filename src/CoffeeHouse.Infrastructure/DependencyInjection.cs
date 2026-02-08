using CoffeeHouse.Domain.Interfaces;
using CoffeeHouse.Application.Interfaces.Security;
using CoffeeHouse.Infrastructure.Identity;
using CoffeeHouse.Infrastructure.Persistence;
using CoffeeHouse.Infrastructure.Persistence.Repositories;
using CoffeeHouse.Infrastructure.Security;
using CoffeeHouse.Application.Interfaces;
using CoffeeHouse.Infrastructure.Caching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeHouse.Infrastructure;

/// <summary>
/// Dependency injection configuration for Infrastructure layer
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Infrastructure layer services to the dependency injection container
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration object</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext
        var connectionString = configuration.GetConnectionString("CoffeeHouseDb");
        services.AddDbContext<CoffeeHouseContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(CoffeeHouseContext).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });

            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(false);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        // Add this line to resolve DbContext for repositories
        services.AddScoped<DbContext>(provider => provider.GetRequiredService<CoffeeHouseContext>());

        // Register repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
        services.AddScoped<ICafeStoreRepository, CafeStoreRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        
        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Security services
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtProvider, JwtProvider>();

        // Register Caching services
        // Register Caching services
        var redisConnectionString = configuration["Redis:ConnectionString"];
        if (!string.IsNullOrEmpty(redisConnectionString) && redisConnectionString != "localhost:6379")
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
            });
        }
        else
        {
            // Fallback to In-Memory Cache if Redis is not configured properly
            services.AddDistributedMemoryCache();
        }
        
        // Register ICacheService implementation which uses IDistributedCache internally
        services.AddScoped<ICacheService, RedisCacheService>();

        return services;
    }
}

