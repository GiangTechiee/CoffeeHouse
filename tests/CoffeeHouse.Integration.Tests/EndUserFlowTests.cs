using CoffeeHouse.Application.Authentication.Commands.Login;
using CoffeeHouse.Application.Authentication.Commands.RegisterCustomer;
using CoffeeHouse.Application.Authentication.DTOs;
using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Products.DTOs;
using CoffeeHouse.Application.Categories.DTOs;
using CoffeeHouse.Application.Orders.DTOs;
using CoffeeHouse.Application.Orders.Commands.CreateOrder;
using CoffeeHouse.Controllers.API;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

using Microsoft.Extensions.DependencyInjection;
using CoffeeHouse.Infrastructure.Persistence;

namespace CoffeeHouse.Integration.Tests
{
    public class EndUserFlowTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public EndUserFlowTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task FullCustomerWorkflow_Succeeds()
        {
            // 1. Register a new customer
            var registerCmd = new RegisterCustomerCommand
            {
                Username = $"customer_{Guid.NewGuid().ToString().Substring(0, 8)}@test.com",
                Password = "UserPassword123!",
                ConfirmPassword = "UserPassword123!",
                FullName = "Test Customer",
                PhoneNumber = "098" + new Random().Next(1000000, 9999999),
                Address = "123 Test St, HCM City"
            };

            var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", registerCmd);
            registerResponse.EnsureSuccessStatusCode();
            var registerResult = await registerResponse.Content.ReadFromJsonAsync<ApiResponse<RegisterResponse>>();
            Assert.NotNull(registerResult?.Data);

            // 2. Login
            var loginCmd = new LoginCommand(registerCmd.Username, registerCmd.Password);
            var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginCmd);
            loginResponse.EnsureSuccessStatusCode();
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
            var token = loginResult?.Data?.Token;
            Assert.NotNull(token);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 3. Browse categories
            var catResponse = await _client.GetAsync("/api/v1/categories");
            catResponse.EnsureSuccessStatusCode();
            var catResult = await catResponse.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<CategoryDto>>>();
            Assert.NotNull(catResult?.Data);

            // 4. Browse products
            var prodResponse = await _client.GetAsync("/api/v1/products");
            prodResponse.EnsureSuccessStatusCode();
            var prodResult = await prodResponse.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<ProductDto>>>();
            Assert.NotNull(prodResult?.Data);
            Assert.NotEmpty(prodResult.Data.Items);

            var product = prodResult.Data.Items.First();

            // 5. Get store (need one to place order)
            int storeId;
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<CoffeeHouseContext>();
                storeId = context.CafeStores.First().StoreId;
            }

            // 6. Place an order
            var orderCmd = new CreateOrderCommand
            {
                CoffeeShopId = storeId,
                PaymentMethod = "Cash",
                Items = new List<CreateOrderItemDto>
                {
                    new() 
                    { 
                        ProductId = product.Id, 
                        ProductName = product.Name, 
                        Quantity = 2, 
                        UnitPrice = product.Price 
                    }
                }
            };

            var orderResponse = await _client.PostAsJsonAsync("/api/v1/orders", orderCmd);
            orderResponse.EnsureSuccessStatusCode();
            var orderResult = await orderResponse.Content.ReadFromJsonAsync<ApiResponse<OrderDto>>();
            Assert.NotNull(orderResult?.Data);
            Assert.Equal("Pending", orderResult.Data.Status);

            // 7. View profile
            var profileResponse = await _client.GetAsync("/api/v1/auth/me");
            profileResponse.EnsureSuccessStatusCode();
            var profileResult = await profileResponse.Content.ReadFromJsonAsync<ApiResponse<UserInfoResponse>>();
            Assert.True(profileResult?.Data?.Name == registerCmd.FullName || profileResult?.Data?.Name == registerCmd.Username);
            
            // 8. View my orders
            var myOrdersResponse = await _client.GetAsync("/api/v1/orders");
            myOrdersResponse.EnsureSuccessStatusCode();
            var myOrdersResult = await myOrdersResponse.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<OrderDto>>>();
            Assert.Contains(myOrdersResult.Data.Items, o => o.OrderId == orderResult.Data.OrderId);
        }
    }
}
