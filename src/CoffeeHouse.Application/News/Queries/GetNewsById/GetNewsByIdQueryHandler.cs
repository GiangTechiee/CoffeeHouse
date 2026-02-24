using CoffeeHouse.Application.News.DTOs;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.News.Queries.GetNewsById;

public class GetNewsByIdQueryHandler : IRequestHandler<GetNewsByIdQuery, NewsArticleDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetNewsByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<NewsArticleDto> Handle(GetNewsByIdQuery request, CancellationToken cancellationToken)
    {
        var article = await _unitOfWork.NewsArticles.GetByIdAsync(request.ArticleId, cancellationToken);
        if (article == null)
        {
            throw new NotFoundException($"NewsArticle with ID {request.ArticleId} not found");
        }

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
