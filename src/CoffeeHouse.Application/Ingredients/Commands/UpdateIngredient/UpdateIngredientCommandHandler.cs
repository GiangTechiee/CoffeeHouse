using CoffeeHouse.Application.Ingredients.DTOs;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.Ingredients.Commands.UpdateIngredient;

public class UpdateIngredientCommandHandler : IRequestHandler<UpdateIngredientCommand, IngredientDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateIngredientCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IngredientDto> Handle(UpdateIngredientCommand request, CancellationToken cancellationToken)
    {
        var ingredient = await _unitOfWork.Ingredients.GetByIdAsync(request.IngredientId, cancellationToken);
        if (ingredient == null)
        {
            throw new NotFoundException($"Ingredient with ID {request.IngredientId} not found");
        }

        ingredient.IngredientName = request.IngredientName.Trim();
        ingredient.Quantity = request.Quantity;
        ingredient.Unit = request.Unit.Trim();
        ingredient.ExpirationDate = request.ExpirationDate;
        ingredient.UnitPrice = request.UnitPrice;
        ingredient.MinimumQuantity = request.MinimumQuantity;

        _unitOfWork.Ingredients.Update(ingredient);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new IngredientDto
        {
            IngredientId = ingredient.IngredientId,
            IngredientName = ingredient.IngredientName,
            Quantity = ingredient.Quantity,
            Unit = ingredient.Unit,
            ExpirationDate = ingredient.ExpirationDate,
            UnitPrice = ingredient.UnitPrice,
            MinimumQuantity = ingredient.MinimumQuantity,
            CreatedAt = ingredient.CreatedAt,
            UpdatedAt = ingredient.UpdatedAt
        };
    }
}
