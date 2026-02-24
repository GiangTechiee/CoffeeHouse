using CoffeeHouse.Application.News.DTOs;
using MediatR;

namespace CoffeeHouse.Application.News.Queries.GetNewsById;

public record GetNewsByIdQuery(int ArticleId) : IRequest<NewsArticleDto>;
