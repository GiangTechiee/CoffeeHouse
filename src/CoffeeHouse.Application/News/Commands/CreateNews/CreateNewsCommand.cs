using CoffeeHouse.Application.News.DTOs;
using MediatR;

namespace CoffeeHouse.Application.News.Commands.CreateNews;

public record CreateNewsCommand(
    string Title,
    string Content,
    DateTime? PublishedAt,
    string? ImageUrl,
    string? Status) : IRequest<NewsArticleDto>;
