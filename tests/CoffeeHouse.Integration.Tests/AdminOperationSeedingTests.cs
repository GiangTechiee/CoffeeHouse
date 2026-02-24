using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.PurchaseOrders.DTOs;
using CoffeeHouse.Application.Orders.DTOs;
using CoffeeHouse.Application.Reports.DTOs;
using CoffeeHouse.Controllers.API.Admin;
using CoffeeHouse.Application.Orders.Commands.CreateOrder;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using CoffeeHouse.Application.CafeStores.DTOs;
using CoffeeHouse.Application.Employees.DTOs;
using CoffeeHouse.Application.Suppliers.DTOs;
using CoffeeHouse.Application.Ingredients.DTOs;
using CoffeeHouse.Application.Products.DTOs;
using CoffeeHouse.Application.Customers.DTOs;

namespace CoffeeHouse.Integration.Tests
{
    public class AdminOperationSeedingTests : BaseIntegrationTest
    {
        public AdminOperationSeedingTests(WebApplicationFactory<Program> factory) : base(factory)
        {
        }

        [Fact]
        public async Task SeedTransactionalData()
        {
            await EnsureAuthenticatedAsync();

            // 1. Fetch needed IDs
            var stores = await GetPaginatedAsync<CafeStoreDto>("/api/v1/admin/cafe-stores");
            var employees = await GetPaginatedAsync<EmployeeDto>("/api/v1/admin/employees");
            var suppliers = await GetPaginatedAsync<SupplierDto>("/api/v1/admin/suppliers");
            var ingredients = await GetPaginatedAsync<IngredientDto>("/api/v1/admin/ingredients");
            var products = await GetPaginatedAsync<ProductDto>("/api/v1/admin/products");
            var customers = await GetPaginatedAsync<CustomerDto>("/api/v1/admin/customers");

            if (stores?.Items.Count == 0 || employees?.Items.Count == 0 || suppliers?.Items.Count == 0 || ingredients?.Items.Count == 0 || products?.Items.Count == 0 || customers?.Items.Count == 0)
            {
                // Core data must be seeded first.
                throw new InvalidOperationException("Core data missing. Run AdminCoreDataSeedingTests first.");
            }

            var storeId = stores.Items.First().StoreId;
            var employeeId = employees.Items.First().Id;
            var supplierId = suppliers.Items.First().SupplierId;
            var ingredientId = ingredients.Items.First().IngredientId;
            var productId = products.Items.First().Id;
            var customerId = customers.Items.First().Id;

            // 2. Seed Purchase Orders (Nhập hàng)
            await SeedPurchaseOrders(storeId, employeeId, supplierId, ingredientId);

            // 3. Seed Sales Orders (Bán hàng)
            await SeedSalesOrders(storeId, employeeId, customerId, productId);

            // 4. Test Report API
            var report = await GetAsync<FinanceSummaryResponse>($"/api/v1/admin/reports/finance?startDate={DateTime.UtcNow.AddMonths(-1):yyyy-MM-dd}&endDate={DateTime.UtcNow:yyyy-MM-dd}");
            Assert.NotNull(report);
        }

        private async Task SeedPurchaseOrders(int storeId, int employeeId, int supplierId, int ingredientId)
        {
            var request = new CreatePurchaseOrderRequest
            {
                StoreId = storeId,
                EmployeeId = employeeId,
                SupplierId = supplierId,
                OrderDate = DateTime.UtcNow,
                Description = "Nhập hàng định kỳ tháng này",
                Status = "Completed",
                Items = new List<PurchaseOrderItemRequest>
                {
                    new() { IngredientId = ingredientId, Quantity = 20, UnitPrice = 85000 }
                }
            };

            var existing = await GetPaginatedAsync<PurchaseOrderListItemDto>("/api/v1/admin/purchase-orders");
            if (existing?.Items.Count == 0)
            {
                await PostAsync<CreatePurchaseOrderRequest, PurchaseOrderDetailDto>("/api/v1/admin/purchase-orders", request);
            }
        }

        private async Task SeedSalesOrders(int storeId, int employeeId, int customerId, int productId)
        {
            var command = new CreateOrderCommand
            {
                CustomerId = customerId,
                CoffeeShopId = storeId,
                PaymentMethod = "Tiền mặt",
                Items = new List<CreateOrderItemDto>
                {
                    new() { ProductId = productId, ProductName = "Sản phẩm test", Quantity = 2, UnitPrice = 30000 }
                }
            };

            var existing = await GetPaginatedAsync<AdminSalesOrderListItemDto>("/api/v1/admin/sales-orders");
            if (existing?.Items.Count == 0)
            {
                // Create via public API
                var created = await PostAsync<CreateOrderCommand, OrderDto>("/api/v1/orders", command);
                
                if (created != null)
                {
                    // Update status to Completed via Admin API
                    var statusRequest = new UpdateSalesOrderStatusRequest
                    {
                        OrderId = created.OrderId,
                        Status = "Hoàn thành",
                        EmployeeId = employeeId
                    };
                    await PutAsync<UpdateSalesOrderStatusRequest, AdminSalesOrderDetailDto>($"/api/v1/admin/sales-orders/{created.OrderId}/status", statusRequest);
                }
            }
        }
    }
}
