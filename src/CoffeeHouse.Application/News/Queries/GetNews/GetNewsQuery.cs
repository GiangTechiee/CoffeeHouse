using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.News.DTOs;
using MediatR;

namespace CoffeeHouse.Application.News.Queries.GetNews;

public record GetNewsQuery(int PageNumber = 1, int PageSize = 20, string? SearchTerm = null)
    : IRequest<PagedResult<NewsArticleDto>>;
