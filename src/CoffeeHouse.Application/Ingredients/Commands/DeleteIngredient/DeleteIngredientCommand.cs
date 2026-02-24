using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.Ingredients.Commands.DeleteIngredient;

public record DeleteIngredientCommand(int IngredientId) : IRequest;

public class DeleteIngredientCommandHandler : IRequestHandler<DeleteIngredientCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteIngredientCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteIngredientCommand request, CancellationToken cancellationToken)
    {
        var ingredient = await _unitOfWork.Ingredients.GetByIdAsync(request.IngredientId, cancellationToken);
        if (ingredient == null)
        {
            throw new NotFoundException($"Ingredient with ID {request.IngredientId} not found");
        }

        _unitOfWork.Ingredients.Delete(ingredient);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
