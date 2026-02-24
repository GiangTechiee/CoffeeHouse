using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Controllers.API;
using CoffeeHouse.Controllers.API.Admin;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using CoffeeHouse.Infrastructure.Persistence;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace CoffeeHouse.Integration.Tests
{
    public abstract class BaseIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        protected readonly WebApplicationFactory<Program> _factory;
        protected readonly HttpClient _client;
        protected string? _adminToken;

        protected BaseIntegrationTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            // Ensure database is created and migrated
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<CoffeeHouseContext>();
                context.Database.EnsureCreated();
            }
        }

        protected async Task EnsureAuthenticatedAsync()
        {
            if (_adminToken != null) return;

            // Use the bootstrap endpoint to ensure we have an admin
            var bootstrapRequest = new BootstrapRequest
            {
                Roles = new List<string> { "Admin", "Employee", "Manager", "User" },
                AdminUser = new AdminUserSeed
                {
                    UserName = "it_admin",
                    Email = "it_admin@coffeehouse.test",
                    Password = "AdminPassword123!",
                    FullName = "Integration Test Admin",
                    Roles = new List<string> { "Admin" }
                }
            };

            // Set the bootstrap token if needed (check Program.cs configuration)
            // For now, assume it works in Dev environment without token or we'll bypass it
            var bootstrapResponse = await _client.PostAsJsonAsync("/api/v1/admin/bootstrap", bootstrapRequest);
            string adminUsername = "it_admin";
            if (bootstrapResponse.IsSuccessStatusCode)
            {
                var bootstrapResult = await bootstrapResponse.Content.ReadFromJsonAsync<ApiResponse<BootstrapResultDto>>();
                if (bootstrapResult?.Data?.AdminUserName != null)
                {
                    adminUsername = bootstrapResult.Data.AdminUserName;
                }
            }
            else
            {
                var bootstrapError = await bootstrapResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Bootstrap Status: {bootstrapResponse.StatusCode}, Body: {bootstrapError}");
            }
            
            var loginRequest = new CoffeeHouse.Application.Authentication.Commands.Login.LoginCommand(adminUsername, "AdminPassword123!");

            var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
            if (loginResponse.IsSuccessStatusCode)
            {
                var result = await loginResponse.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
                _adminToken = result?.Data?.Token;
            }
            else
            {
                var errorBody = await loginResponse.Content.ReadAsStringAsync();
                throw new Exception($"Failed to authenticate as Admin. Status: {loginResponse.StatusCode}, Body: {errorBody}");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);
        }

        protected async Task<T?> GetAsync<T>(string url)
        {
            await EnsureAuthenticatedAsync();
            var response = await _client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception($"GET {url} failed with {response.StatusCode}. Body: {body}");
            }
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
            return result != null ? result.Data : default;
        }

        protected async Task<PaginatedResponse<T>?> GetPaginatedAsync<T>(string url)
        {
            await EnsureAuthenticatedAsync();
            var response = await _client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception($"GET {url} failed with {response.StatusCode}. Body: {body}");
            }
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<T>>>();
            return result != null ? result.Data : default;
        }

        protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest request)
        {
            await EnsureAuthenticatedAsync();
            var response = await _client.PostAsJsonAsync(url, request);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception($"POST {url} failed with {response.StatusCode}. Body: {body}");
            }
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<TResponse>>();
            return result != null ? result.Data : default;
        }

        protected async Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest request)
        {
            await EnsureAuthenticatedAsync();
            var response = await _client.PutAsJsonAsync(url, request);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception($"PUT {url} failed with {response.StatusCode}. Body: {body}");
            }
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<TResponse>>();
            return result != null ? result.Data : default;
        }

        protected async Task DeleteAsync(string url)
        {
            await EnsureAuthenticatedAsync();
            var response = await _client.DeleteAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception($"DELETE {url} failed with {response.StatusCode}. Body: {body}");
            }
        }
    }
}
