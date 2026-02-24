using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.News.Commands.DeleteNews;

public record DeleteNewsCommand(int ArticleId) : IRequest;

public class DeleteNewsCommandHandler : IRequestHandler<DeleteNewsCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteNewsCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteNewsCommand request, CancellationToken cancellationToken)
    {
        var article = await _unitOfWork.NewsArticles.GetByIdAsync(request.ArticleId, cancellationToken);
        if (article == null)
        {
            throw new NotFoundException($"NewsArticle with ID {request.ArticleId} not found");
        }

        _unitOfWork.NewsArticles.Delete(article);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
