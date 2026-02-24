using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.News.DTOs;
using MediatR;

namespace CoffeeHouse.Application.News.Queries.GetPublishedNews;

public record GetPublishedNewsQuery(int PageNumber = 1, int PageSize = 20, string? SearchTerm = null)
    : IRequest<PagedResult<NewsArticleDto>>;
