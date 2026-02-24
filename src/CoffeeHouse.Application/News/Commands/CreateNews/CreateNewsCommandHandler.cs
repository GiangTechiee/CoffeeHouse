using CoffeeHouse.Application.News.DTOs;
using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.News.Commands.CreateNews;

public class CreateNewsCommandHandler : IRequestHandler<CreateNewsCommand, NewsArticleDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateNewsCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<NewsArticleDto> Handle(CreateNewsCommand request, CancellationToken cancellationToken)
    {
        var article = new NewsArticle
        {
            Title = request.Title.Trim(),
            Content = request.Content.Trim(),
            ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim(),
            Status = string.IsNullOrWhiteSpace(request.Status) ? "Draft" : request.Status.Trim(),
            PublishedAt = request.PublishedAt ?? DateTime.UtcNow
        };

        await _unitOfWork.NewsArticles.AddAsync(article, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new NewsArticleDto
        {
            ArticleId = article.ArticleId,
            Title = article.Title,
            Content = article.Content,
            ImageUrl = article.ImageUrl,
            Status = article.Status,
            PublishedAt = article.PublishedAt
        };
    }
}
