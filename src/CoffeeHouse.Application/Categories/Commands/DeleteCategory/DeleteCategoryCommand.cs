
using CoffeeHouse.Domain.Interfaces;
using CoffeeHouse.Domain.Exceptions;
using MediatR;

namespace CoffeeHouse.Application.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(int Id) : IRequest;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.ProductCategories.GetByIdAsync(request.Id);

        if (category == null)
        {
            throw new NotFoundException($"ProductCategory with ID {request.Id} not found");
        }

        _unitOfWork.ProductCategories.Delete(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
