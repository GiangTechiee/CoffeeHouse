using CoffeeHouse.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CoffeeHouse.Infrastructure.Persistence
{
    public class CoffeeHouseContextFactory : IDesignTimeDbContextFactory<CoffeeHouseContext>
    {
        public CoffeeHouseContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CoffeeHouseContext>();
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("CoffeeHouseDb");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Missing ConnectionStrings:CoffeeHouseDb. Configure a Neon connection string.");
            }

            if (connectionString.Contains("localhost", StringComparison.OrdinalIgnoreCase) ||
                connectionString.Contains("127.0.0.1", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Local database connection strings are not allowed. Use Neon.");
            }

            optionsBuilder.UseNpgsql(connectionString);

            return new CoffeeHouseContext(optionsBuilder.Options, null!); 
        }
    }
}
