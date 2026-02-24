using CoffeeHouse.Application.News.DTOs;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.News.Commands.UpdateNews;

public class UpdateNewsCommandHandler : IRequestHandler<UpdateNewsCommand, NewsArticleDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateNewsCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<NewsArticleDto> Handle(UpdateNewsCommand request, CancellationToken cancellationToken)
    {
        var article = await _unitOfWork.NewsArticles.GetByIdAsync(request.ArticleId, cancellationToken);
        if (article == null)
        {
            throw new NotFoundException($"NewsArticle with ID {request.ArticleId} not found");
        }

        article.Title = request.Title.Trim();
        article.Content = request.Content.Trim();
        article.ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim();
        article.Status = string.IsNullOrWhiteSpace(request.Status) ? article.Status : request.Status.Trim();
        if (request.PublishedAt.HasValue)
        {
            article.PublishedAt = request.PublishedAt.Value;
        }

        _unitOfWork.NewsArticles.Update(article);
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
