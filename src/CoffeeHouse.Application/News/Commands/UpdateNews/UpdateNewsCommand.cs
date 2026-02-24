using CoffeeHouse.Application.News.DTOs;
using MediatR;

namespace CoffeeHouse.Application.News.Commands.UpdateNews;

public record UpdateNewsCommand(
    int ArticleId,
    string Title,
    string Content,
    DateTime? PublishedAt,
    string? ImageUrl,
    string? Status) : IRequest<NewsArticleDto>;
