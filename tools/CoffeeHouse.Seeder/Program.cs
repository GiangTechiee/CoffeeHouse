using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Identity;
using CoffeeHouse.Infrastructure.Persistence;
using Flurl.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CoffeeHouse.Seeder;

class Program
{
    private static IServiceProvider ServiceProvider = null!;
    private static string BaseUrl = "http://localhost:5069"; 
    private static string ConnectionString = "";

    static async Task Main(string[] args)
    {
        Console.WriteLine("Initializing Seeder...");

        // Setup Configuration
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();
        ConnectionString = configuration.GetConnectionString("CoffeeHouseDb") ?? "";

        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            Console.WriteLine("Missing ConnectionStrings:CoffeeHouseDb. Configure a Neon connection string.");
            return;
        }

        if (ConnectionString.Contains("localhost", StringComparison.OrdinalIgnoreCase) ||
            ConnectionString.Contains("127.0.0.1", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Local database connection strings are not allowed. Use Neon.");
            return;
        }

        Console.WriteLine($"Using connection string: {ConnectionString}");
        Console.WriteLine($"Using API URL: {BaseUrl}");

        // Setup DI for EF Core
        var services = new ServiceCollection();
        services.AddLogging(configure => configure.AddConsole());
        services.AddDbContext<CoffeeHouseContext>(options =>
            options.UseNpgsql(ConnectionString));

        services.AddIdentity<AppUser, AppRole>(options =>
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

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

        ServiceProvider = services.BuildServiceProvider();

        try
        {
            // 1. Ensure Store Exists
            int storeId = await EnsureStoreExists();

            // 2. Ensure Admin User Exists (Direct DB access)
            string adminToken = await EnsureAdminUserAndLogin(storeId);
            
            // Allow processing to continue for debugging purposes even if admin token is missing, 
            // but SeedCategories will likely fail.
            if (!string.IsNullOrEmpty(adminToken))
            {
            // 3. Seed Categories (API)
            await SeedCategories(adminToken);

            // 4. Seed Products (API)
            await SeedProducts(adminToken);
            }
            else
            {
                Console.WriteLine("Skipping category/product seeding due to missing admin token.");
            }

            // 5. Register Customer (API)
            var (customerUser, customerToken, customerId) = await RegisterCustomer();

            if (!string.IsNullOrEmpty(customerToken))
            {
                // 6. Place Orders (API)
                await PlaceOrder(customerToken, storeId, customerId);
            }

            Console.WriteLine("Seeding completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }

    private static async Task<int> EnsureStoreExists()
    {
        Console.WriteLine("Ensuring CafeStore exists...");
        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CoffeeHouseContext>();

        var store = await context.CafeStores.FirstOrDefaultAsync();
        if (store == null)
        {
            store = new CafeStore
            {
                StoreName = "Main Branch",
                Address = "123 Coffee Street",
                PhoneNumber = "0900000000",
                Email = "store@coffeehouse.com"
            };
            context.CafeStores.Add(store);
            await context.SaveChangesAsync();
            Console.WriteLine("Created Main Branch Store.");
        }
        return store.StoreId;
    }

    private static async Task<string> EnsureAdminUserAndLogin(int storeId)
    {
        Console.WriteLine("Ensuring Admin user exists...");
        var adminEmail = "admin@coffeehouse.com";
        var password = "Admin@123";

        using var scope = ServiceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        
        // Ensure roles exist
        string[] roles = { "Admin", "Manager", "Employee", "User" };
        foreach (var role in roles)
        {
             if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new AppRole { Name = role });
            }
        }

        // Check if user exists (by email or username)
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = await userManager.FindByNameAsync(adminEmail);
        }
        
        if (adminUser == null)
        {
             Console.WriteLine("Admin user not found. Registering via API...");
             // Check by username "admin" and delete if exists (cleanup)
            var existingUserByName = await userManager.FindByNameAsync("admin");
            if (existingUserByName != null)
            {
                 await userManager.DeleteAsync(existingUserByName);
            }

            try 
            {
                await $"{BaseUrl}/api/v1/auth/register"
                    .PostJsonAsync(new 
                    {
                        Username = adminEmail,
                        Email = adminEmail,
                        Password = password,
                        ConfirmPassword = password,
                        FullName = "System Administrator",
                        PhoneNumber = "0999999999",
                        Address = "123 Admin Street"
                    });
                Console.WriteLine("Registered Admin via API.");
                
                // Refresh adminUser from DB
                adminUser = await userManager.FindByEmailAsync(adminEmail) 
                            ?? await userManager.FindByNameAsync(adminEmail);
            }
            catch (FlurlHttpException ex)
            {
                 Console.WriteLine($"API Registration failed: {ex.Message}");
                 if (ex.Call?.Response != null)
                     Console.WriteLine(await ex.Call.Response.GetStringAsync());
                 
                 // If user already exists, try to load it and continue
                 adminUser = await userManager.FindByEmailAsync(adminEmail) 
                             ?? await userManager.FindByNameAsync(adminEmail);
                 if (adminUser == null)
                 {
                     return "";
                 }
            }
        }

        // Ensure Admin Role
        if (adminUser != null)
        {
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine("Assigned Admin role to user.");
            }
            
            // Ensure Employee Link
            if (adminUser.EmployeeId == null)
            {
                var context = scope.ServiceProvider.GetRequiredService<CoffeeHouseContext>();
                // check if employee exists
                 var emp = await context.Employees.FirstOrDefaultAsync(e => e.Email == adminEmail);
                 if (emp == null)
                 {
                    emp = new Employee
                    {
                        FullName = "System Administrator",
                        Position = "Administrator",
                        StoreId = storeId,
                        IdentityCardNumber = "000000000000",
                        PhoneNumber = "0999999999",
                        BaseSalary = 10000000,
                        SalaryCoefficient = 1.0m,
                        Email = adminEmail
                    };
                    context.Employees.Add(emp);
                    await context.SaveChangesAsync();
                 }
                
                adminUser.EmployeeId = emp.EmployeeId;
                await userManager.UpdateAsync(adminUser);
                Console.WriteLine("Linked Admin to Employee record.");
            }

            // Ensure known password for login (reset without old password)
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(adminUser);
            var resetResult = await userManager.ResetPasswordAsync(adminUser, resetToken, password);
            if (!resetResult.Succeeded)
            {
                Console.WriteLine("Failed to reset admin password. Errors:");
                foreach (var err in resetResult.Errors)
                {
                    Console.WriteLine($" - {err.Code}: {err.Description}");
                }
            }
        }

        // Login to get token (now guaranteed to have Admin role)
        Console.WriteLine("Logging in as Admin...");
        try 
        {
            var response = await $"{BaseUrl}/api/v1/auth/login"
                .PostJsonAsync(new { Username = adminEmail, Password = password })
                .ReceiveJson<ApiResponse<LoginResponseData>>();
            
            Console.WriteLine($"Admin logged in. Token length: {response?.Data?.Token?.Length ?? 0}");
            return response?.Data?.Token ?? "";
        }
        catch (FlurlHttpException ex)
        {
            Console.WriteLine($"Login failed: {ex.Message}");
            return "";
        }
    }

    private static async Task SeedCategories(string token)
    {
        Console.WriteLine("Seeding Categories...");
        var categories = new[]
        {
            new { Name = "Coffee", Description = "Clean roasted coffee beans" },
            new { Name = "Tea", Description = "Premium tea leaves" },
            new { Name = "Smoothie", Description = "Fresh fruit smoothies" },
            new { Name = "Cake", Description = "Delicious pastries" }
        };

        foreach (var cat in categories)
        {
            try
            {
                await $"{BaseUrl}/api/v1/categories"
                    .WithOAuthBearerToken(token)
                    .PostJsonAsync(cat);
                
                Console.WriteLine($"Created category: {cat.Name}");
            }
            catch (FlurlHttpException ex)
            {
                 // Ignore if duplicate
                 Console.WriteLine($"Failed to create category {cat.Name}: {ex.Message}");
            }
        }
    }

    private static async Task SeedProducts(string token)
    {
        Console.WriteLine("Seeding Products...");
        
        // Hardcoding some products for demonstration
        var products = new[]
        {
            new { Name = "Espresso", Price = 20000, Category = "Coffee", Desc = "Strong black coffee" },
            new { Name = "Cappuccino", Price = 35000, Category = "Coffee", Desc = "Espresso with milk foam" },
            new { Name = "Peach Tea", Price = 30000, Category = "Tea", Desc = "Refreshing peach tea" },
            new { Name = "Cheese Cake", Price = 45000, Category = "Cake", Desc = "Creamy cheese cake" }
        };

        var categoryIds = await GetCategoryIdMap();

        foreach (var p in products)
        {
             try {
                if (!categoryIds.TryGetValue(p.Category, out var categoryId))
                {
                    Console.WriteLine($"Skipping product {p.Name}: category '{p.Category}' not found.");
                    continue;
                }

                await $"{BaseUrl}/api/v1/products"
                    .WithOAuthBearerToken(token)
                    .PostJsonAsync(new 
                    {
                        ProductName = p.Name,
                        Price = p.Price,
                        Description = p.Desc,
                        CategoryId = categoryId 
                    });
                Console.WriteLine($"Created product: {p.Name}");
             } catch (FlurlHttpException ex) {
                 Console.WriteLine($"Failed to create product {p.Name}: {ex.Message}");
                 if (ex.Call?.Response != null)
                    Console.WriteLine(await ex.Call.Response.GetStringAsync());
             }
        }
    }

    private static async Task<(string, string, int)> RegisterCustomer()
    {
        Console.WriteLine("Registering Customer...");
        // Use email format for username to satisfy validation
        var email = $"customer_{DateTime.Now.Ticks}@example.com";
        var password = "Customer@123";
        
        try
        {
            await $"{BaseUrl}/api/v1/auth/register"
                .PostJsonAsync(new 
                {
                    Username = email,
                    Email = email,
                    Password = password,
                    ConfirmPassword = password,
                    FullName = "Test Customer",
                    PhoneNumber = "0123456789",
                    Address = "123 Test Street"
                });
            
            Console.WriteLine($"Registered customer: {email}");

            // Login
            var response = await $"{BaseUrl}/api/v1/auth/login"
                .PostJsonAsync(new { Username = email, Password = password })
                .ReceiveJson<ApiResponse<LoginResponseData>>();
            
            // Get CustomerId from DB
            using var scope = ServiceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var user = await userManager.FindByNameAsync(email);
            var customerId = user?.CustomerId ?? 0;

            return (email, response?.Data?.Token ?? "", customerId);
        }
        catch (FlurlHttpException ex)
        {
            Console.WriteLine($"Customer registration/login failed: {ex.Message}");
             if (ex.Call?.Response != null)
                Console.WriteLine(await ex.Call.Response.GetStringAsync());
            return ("", "", 0);
        }
    }

    private static async Task PlaceOrder(string token, int storeId, int customerId)
    {
        Console.WriteLine("Placing Order...");

        var productIds = await GetProductIdMap();
        if (!productIds.TryGetValue("Espresso", out var espressoId) ||
            !productIds.TryGetValue("Cappuccino", out var cappuccinoId))
        {
            Console.WriteLine("Skipping order placement: required products not found.");
            return;
        }

        var order = new 
        {
            CoffeeShopId = storeId,
            CustomerId = customerId,
            PaymentMethod = "Cash",
            Items = new[]
            {
                new { ProductId = espressoId, ProductName = "Espresso", Quantity = 2, UnitPrice = 20000 },
                new { ProductId = cappuccinoId, ProductName = "Cappuccino", Quantity = 1, UnitPrice = 35000 }
            }
        };

        try
        {
            await $"{BaseUrl}/api/v1/orders"
                .WithOAuthBearerToken(token)
                .PostJsonAsync(order);
            Console.WriteLine("Order placed successfully.");
        }
        catch (FlurlHttpException ex)
        {
            Console.WriteLine($"Order placement failed: {ex.Message}");
             if (ex.Call?.Response != null)
                Console.WriteLine(await ex.Call.Response.GetStringAsync());
        }
    }

    private static async Task<Dictionary<string, int>> GetCategoryIdMap()
    {
        try
        {
            var json = await $"{BaseUrl}/api/v1/categories?page=1&pageSize=100"
                .GetStringAsync();

            using var doc = JsonDocument.Parse(json);
            var items = doc.RootElement.GetProperty("data").GetProperty("items");
            var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in items.EnumerateArray())
            {
                var name = item.GetProperty("name").GetString();
                var id = item.GetProperty("id").GetInt32();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    map[name] = id;
                }
            }
            return map;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load categories: {ex.Message}");
            return new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }
    }

    private static async Task<Dictionary<string, int>> GetProductIdMap()
    {
        try
        {
            var json = await $"{BaseUrl}/api/v1/products?page=1&pageSize=200"
                .GetStringAsync();

            using var doc = JsonDocument.Parse(json);
            var items = doc.RootElement.GetProperty("data").GetProperty("items");
            var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in items.EnumerateArray())
            {
                var name = item.GetProperty("name").GetString();
                var id = item.GetProperty("id").GetInt32();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    map[name] = id;
                }
            }
            return map;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load products: {ex.Message}");
            return new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }
    }
}

class LoginResponseData 
{ 
    public string? Token { get; set; } 
    // Also likely has CustomerId etc but ignoring 
}
class ApiResponse<T> 
{ 
    public bool Success { get; set; }
    public T? Data { get; set; } 
    public object? Error { get; set; }
}
class CategoryDto 
{ 
    public int CategoryId { get; set; } 
    public string? CategoryName { get; set; } 
}
