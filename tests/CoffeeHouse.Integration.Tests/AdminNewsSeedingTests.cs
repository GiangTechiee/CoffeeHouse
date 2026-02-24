using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.News.DTOs;
using CoffeeHouse.Controllers.API.Admin;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CoffeeHouse.Integration.Tests
{
    public class AdminNewsSeedingTests : BaseIntegrationTest
    {
        public AdminNewsSeedingTests(WebApplicationFactory<Program> factory) : base(factory)
        {
        }

        [Fact]
        public async Task SeedNewsArticles()
        {
            await EnsureAuthenticatedAsync();

            var articlesToSeed = new List<CreateNewsArticleRequest>
            {
                new() 
                { 
                    Title = "Khai trương chi nhánh Phan Xích Long", 
                    Content = "CoffeeHouse vui mừng thông báo khai trương chi nhánh mới tại 123 Phan Xích Long với nhiều ưu đãi hấp dẫn...", 
                    Status = "Published",
                    PublishedAt = DateTime.UtcNow
                },
                new() 
                { 
                    Title = "Hương vị cà phê mới: Arabica Cầu Đất", 
                    Content = "Khám phá hương vị thanh tao, chua nhẹ đặc trưng của dòng cà phê Arabica thượng hạng từ vùng đất Cầu Đất, Đà Lạt.", 
                    Status = "Published",
                    PublishedAt = DateTime.UtcNow
                },
                new() 
                { 
                    Title = "Chương trình khuyến mãi Mùa Hè Rực Rỡ", 
                    Content = "Mua 1 tặng 1 cho tất các các dòng trà trái cây và đá xay vào mỗi khung giờ vàng 14h-17h hàng ngày.", 
                    Status = "Published",
                    PublishedAt = DateTime.UtcNow
                }
            };

            var existing = await GetPaginatedAsync<NewsArticleDto>("/api/v1/admin/news");
            var existingTitles = existing?.Items.Select(a => a.Title).ToHashSet() ?? new HashSet<string>();

            foreach (var article in articlesToSeed)
            {
                if (!existingTitles.Contains(article.Title))
                {
                    await PostAsync<CreateNewsArticleRequest, NewsArticleDto>("/api/v1/admin/news", article);
                }
            }
        }
    }
}
