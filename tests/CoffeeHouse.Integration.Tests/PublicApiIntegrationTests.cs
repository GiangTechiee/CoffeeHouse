using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Products.DTOs;
using CoffeeHouse.Application.News.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using System.Net.Http.Json;

namespace CoffeeHouse.Integration.Tests
{
    public class PublicApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public PublicApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task TestPublicEndpoints()
        {
            // 1. Get Products
            var productsResponse = await _client.GetAsync("/api/v1/products");
            productsResponse.EnsureSuccessStatusCode();
            var productsResult = await productsResponse.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<ProductDto>>>();
            Assert.True(productsResult?.Success);
            Assert.NotNull(productsResult?.Data);

            // 2. Get News
            var newsResponse = await _client.GetAsync("/api/v1/news");
            newsResponse.EnsureSuccessStatusCode();
            var newsResult = await newsResponse.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<NewsArticleDto>>>();
            Assert.True(newsResult?.Success);

            // 3. Search Products
            var searchResponse = await _client.GetAsync("/api/v1/products?search=Cà phê");
            searchResponse.EnsureSuccessStatusCode();
            var searchResult = await searchResponse.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<ProductDto>>>();
            Assert.True(searchResult?.Success);
        }
    }
}
