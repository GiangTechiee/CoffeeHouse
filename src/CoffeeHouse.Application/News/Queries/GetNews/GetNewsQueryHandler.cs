using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.News.DTOs;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.News.Queries.GetNews;

public class GetNewsQueryHandler : IRequestHandler<GetNewsQuery, PagedResult<NewsArticleDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetNewsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<NewsArticleDto>> Handle(GetNewsQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : request.PageSize;
        if (pageSize > 100) pageSize = 100;

        var (items, totalCount) = await _unitOfWork.NewsArticles.GetPagedAsync(
            request.SearchTerm,
            pageNumber,
            pageSize,
            cancellationToken);

        var dtos = items.Select(MapToDto).ToList();

        return new PagedResult<NewsArticleDto>
        {
            Items = dtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    private static NewsArticleDto MapToDto(Domain.Entities.NewsArticle article)
    {
        return new NewsArticleDto
        {
            ArticleId = article.ArticleId,
            Title = article.Title,
            PublishedAt = article.PublishedAt,
            Content = article.Content,
            ImageUrl = article.ImageUrl,
            Status = article.Status
        };
    }
}
