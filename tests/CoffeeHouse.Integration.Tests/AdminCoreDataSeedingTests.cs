using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Suppliers.DTOs;
using CoffeeHouse.Application.Ingredients.DTOs;
using CoffeeHouse.Application.Categories.DTOs;
using CoffeeHouse.Application.Products.DTOs;
using CoffeeHouse.Application.Employees.DTOs;
using CoffeeHouse.Application.Customers.DTOs;
using CoffeeHouse.Controllers.API.Admin;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using CoffeeHouse.Application.CafeStores.DTOs;

namespace CoffeeHouse.Integration.Tests
{
    public class AdminCoreDataSeedingTests : BaseIntegrationTest
    {
        public AdminCoreDataSeedingTests(WebApplicationFactory<Program> factory) : base(factory)
        {
        }

        [Fact]
        public async Task SeedAllCoreData()
        {
            await EnsureAuthenticatedAsync();

            // 1. Seed Cafe Stores
            var stores = await SeedCafeStores();
            var mainStoreId = stores.First().StoreId;

            // 2. Seed Suppliers
            var suppliers = await SeedSuppliers();

            // 3. Seed Ingredients
            var ingredients = await SeedIngredients();

            // 4. Seed Product Categories
            var categories = await SeedProductCategories();

            // 5. Seed Products
            await SeedProducts(categories);

            // 6. Seed Employees
            await SeedEmployees(mainStoreId);

            // 7. Seed Customers
            await SeedCustomers();
        }

        private async Task<List<CafeStoreDto>> SeedCafeStores()
        {
            var storesToSeed = new List<CreateCafeStoreRequest>
            {
                new() { StoreName = "CoffeeHouse - Phan Xích Long", Address = "123 Phan Xích Long, Phú Nhuận, TP.HCM", PhoneNumber = "02838451234", Email = "pxl@coffeehouse.com" },
                new() { StoreName = "CoffeeHouse - Nguyễn Huệ", Address = "45 Nguyễn Huệ, Quận 1, TP.HCM", PhoneNumber = "02838215678", Email = "nguyenhue@coffeehouse.com" },
                new() { StoreName = "CoffeeHouse - Hồ Con Rùa", Address = "2 Công Trường Quốc Tế, Quận 3, TP.HCM", PhoneNumber = "02838290909", Email = "hoconrua@coffeehouse.com" }
            };

            var existing = await GetPaginatedAsync<CafeStoreDto>("/api/v1/admin/cafe-stores");
            var existingNames = existing?.Items.Select(s => s.StoreName).ToHashSet() ?? new HashSet<string>();

            var result = new List<CafeStoreDto>();
            foreach (var store in storesToSeed)
            {
                if (!existingNames.Contains(store.StoreName))
                {
                    var created = await PostAsync<CreateCafeStoreRequest, CafeStoreDto>("/api/v1/admin/cafe-stores", store);
                    if (created != null) result.Add(created);
                }
            }

            return result.Count > 0 ? result : (existing?.Items ?? new List<CafeStoreDto>());
        }

        private async Task<List<SupplierDto>> SeedSuppliers()
        {
            var suppliersToSeed = new List<CreateSupplierRequest>
            {
                new() { SupplierName = "Nông sản Đà Lạt - DalatGap", Address = "Đà Lạt, Lâm Đồng", PhoneNumber = "02633123456", Status = "Active", Stk = "000111222333" },
                new() { SupplierName = "Vinamilk - Chi nhánh TP.HCM", Address = "Quận 7, TP.HCM", PhoneNumber = "02854155555", Status = "Active", Stk = "111222333444" },
                new() { SupplierName = "Trung Nguyên Legend - Raw Material", Address = "Buôn Ma Thuột, Đắk Lắk", PhoneNumber = "02621234567", Status = "Active", Stk = "222333444555" }
            };

            var existing = await GetPaginatedAsync<SupplierDto>("/api/v1/admin/suppliers");
            var existingNames = existing?.Items.Select(s => s.SupplierName).ToHashSet() ?? new HashSet<string>();

            var result = new List<SupplierDto>();
            foreach (var supplier in suppliersToSeed)
            {
                if (!existingNames.Contains(supplier.SupplierName))
                {
                    var created = await PostAsync<CreateSupplierRequest, SupplierDto>("/api/v1/admin/suppliers", supplier);
                    if (created != null) result.Add(created);
                }
            }
            return result.Count > 0 ? result : (existing?.Items ?? new List<SupplierDto>());
        }

        private async Task<List<IngredientDto>> SeedIngredients()
        {
            var itemsToSeed = new List<CreateIngredientRequest>
            {
                new() { IngredientName = "Hạt Robusta Đắk Lắk", Quantity = 100, Unit = "kg", UnitPrice = 85000, MinimumQuantity = 10, ExpirationDate = DateTime.UtcNow.AddMonths(12) },
                new() { IngredientName = "Hạt Arabica Cầu Đất", Quantity = 50, Unit = "kg", UnitPrice = 145000, MinimumQuantity = 5, ExpirationDate = DateTime.UtcNow.AddMonths(12) },
                new() { IngredientName = "Sữa đặc Ngôi Sao Phương Nam", Quantity = 120, Unit = "hộp", UnitPrice = 18500, MinimumQuantity = 20, ExpirationDate = DateTime.UtcNow.AddMonths(6) },
                new() { IngredientName = "Sữa tươi Barista", Quantity = 60, Unit = "lít", UnitPrice = 32000, MinimumQuantity = 10, ExpirationDate = DateTime.UtcNow.AddDays(15) }
            };

            var existing = await GetPaginatedAsync<IngredientDto>("/api/v1/admin/ingredients");
            var existingNames = existing?.Items.Select(s => s.IngredientName).ToHashSet() ?? new HashSet<string>();

            var result = new List<IngredientDto>();
            foreach (var item in itemsToSeed)
            {
                if (!existingNames.Contains(item.IngredientName))
                {
                    var created = await PostAsync<CreateIngredientRequest, IngredientDto>("/api/v1/admin/ingredients", item);
                    if (created != null) result.Add(created);
                }
            }
            return result.Count > 0 ? result : (existing?.Items ?? new List<IngredientDto>());
        }

        private async Task<List<CategoryDto>> SeedProductCategories()
        {
            var categoriesToSeed = new List<CreateCategoryRequest>
            {
                new() { Name = "Cà phê Việt Nam", Description = "Các món cà phê truyền thống Việt Nam" },
                new() { Name = "Cà phê Máy", Description = "Espresso, Latte, Cappuccino..." },
                new() { Name = "Trà & Trái Cây", Description = "Các loại trà trái cây tươi mát" },
                new() { Name = "Đá Xay", Description = "Các món đá xay mát lạnh" },
                new() { Name = "Bánh Ngọt", Description = "Bánh mì, bánh ngọt ăn kèm" }
            };

            var existing = await GetPaginatedAsync<CategoryDto>("/api/v1/admin/product-categories");
            var existingNames = existing?.Items.Select(s => s.Name).ToHashSet() ?? new HashSet<string>();

            var result = new List<CategoryDto>();
            foreach (var cat in categoriesToSeed)
            {
                if (!existingNames.Contains(cat.Name))
                {
                    var created = await PostAsync<CreateCategoryRequest, CategoryDto>("/api/v1/admin/product-categories", cat);
                    if (created != null) result.Add(created);
                }
            }
            return result.Count > 0 ? result : (existing?.Items ?? new List<CategoryDto>());
        }

        private async Task SeedProducts(List<CategoryDto> categories)
        {
            var vnCoffeeId = categories.FirstOrDefault(c => c.Name == "Cà phê Việt Nam")?.Id ?? categories.First().Id;
            var machineCoffeeId = categories.FirstOrDefault(c => c.Name == "Cà phê Máy")?.Id ?? categories.First().Id;

            var productsToSeed = new List<CoffeeHouse.Application.Products.Commands.CreateProduct.CreateProductCommand>
            {
                new() { Name = "Cà Phê Đen Đá", Price = 29000, Description = "Cà phê đậm đà phong cách truyền thống", CategoryId = vnCoffeeId },
                new() { Name = "Cà Phê Sữa Đá", Price = 35000, Description = "Sự kết hợp hoàn hảo giữa cà phê và sữa đặc", CategoryId = vnCoffeeId },
                new() { Name = "Bạc Xỉu", Price = 39000, Description = "Món uống quen thuộc của người Sài Gòn", CategoryId = vnCoffeeId },
                new() { Name = "Latte Coffee", Price = 55000, Description = "Espresso cùng sữa nóng và lớp bọt mịn", CategoryId = machineCoffeeId },
                new() { Name = "Cappuccino", Price = 55000, Description = "Hương vị espresso hòa quyện cùng bọt sữa", CategoryId = machineCoffeeId }
            };

            var existing = await GetPaginatedAsync<ProductDto>("/api/v1/admin/products");
            var existingNames = existing?.Items.Select(s => s.Name).ToHashSet() ?? new HashSet<string>();

            foreach (var product in productsToSeed)
            {
                if (!existingNames.Contains(product.Name))
                {
                    await PostAsync<CoffeeHouse.Application.Products.Commands.CreateProduct.CreateProductCommand, ProductDto>("/api/v1/admin/products", product);
                }
            }
        }

        private async Task SeedEmployees(int storeId)
        {
            var employeesToSeed = new List<CoffeeHouse.Application.Employees.Commands.CreateEmployee.CreateEmployeeCommand>
            {
                new() { FullName = "Nguyễn Văn A", Position = "Quản lý", CoffeeShopId = storeId, Email = "vana@coffeehouse.com", PhoneNumber = "0901112233", IdCardNumber = "079012345678", BaseSalary = 15000000, SalaryCoefficient = 1.2M },
                new() { FullName = "Trần Thị B", Position = "Pha chế", CoffeeShopId = storeId, Email = "thib@coffeehouse.com", PhoneNumber = "0902223344", IdCardNumber = "079087654321", BaseSalary = 8000000, SalaryCoefficient = 1.0M },
                new() { FullName = "Lê Văn C", Position = "Phục vụ", CoffeeShopId = storeId, Email = "vanc@coffeehouse.com", PhoneNumber = "0903334455", IdCardNumber = "079011223344", BaseSalary = 6000000, SalaryCoefficient = 1.0M }
            };

            var existing = await GetPaginatedAsync<EmployeeDto>("/api/v1/admin/employees");
            var existingNames = existing?.Items.Select(s => s.FullName).ToHashSet() ?? new HashSet<string>();

            foreach (var emp in employeesToSeed)
            {
                if (!existingNames.Contains(emp.FullName))
                {
                    await PostAsync<CoffeeHouse.Application.Employees.Commands.CreateEmployee.CreateEmployeeCommand, EmployeeDto>("/api/v1/admin/employees", emp);
                }
            }
        }

        private async Task SeedCustomers()
        {
            var customersToSeed = new List<CreateCustomerRequest>
            {
                new() { Name = "Anh Hoàng", PhoneNumber = "0988001122", Address = "Quận 1, TP.HCM" },
                new() { Name = "Chị Mai", PhoneNumber = "0988334455", Address = "Quận Phú Nhuận, TP.HCM" },
                new() { Name = "Bác Hùng", PhoneNumber = "0988667788", Address = "Quận Bình Thạnh, TP.HCM" }
            };

            var existing = await GetPaginatedAsync<CustomerDto>("/api/v1/admin/customers");
            var existingNames = existing?.Items.Select(s => s.Name).ToHashSet() ?? new HashSet<string>();

            foreach (var customer in customersToSeed)
            {
                if (!existingNames.Contains(customer.Name))
                {
                    await PostAsync<CreateCustomerRequest, CustomerDto>("/api/v1/admin/customers", customer);
                }
            }
        }
    }
}
