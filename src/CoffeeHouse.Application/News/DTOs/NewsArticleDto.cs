namespace CoffeeHouse.Application.News.DTOs;

public class NewsArticleDto
{
    public int ArticleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Status { get; set; } = string.Empty;
}
