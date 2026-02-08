using System.Reflection;
using CoffeeHouse.Application.Common.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeHouse.Application;

/// <summary>
/// Dependency injection configuration for Application layer
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Application layer services to the dependency injection container
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register AutoMapper with all profiles from this assembly
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // Register MediatR with pipeline behaviors
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            
            // Add validation behavior to pipeline
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            
            // Add logging behavior to pipeline
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));

            // Add cache behavior to pipeline
            cfg.AddOpenBehavior(typeof(CacheBehavior<,>));
        });

        // Register FluentValidation validators from this assembly
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
