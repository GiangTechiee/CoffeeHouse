using CoffeeHouse.Application.Ingredients.DTOs;
using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.Ingredients.Commands.CreateIngredient;

public class CreateIngredientCommandHandler : IRequestHandler<CreateIngredientCommand, IngredientDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateIngredientCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IngredientDto> Handle(CreateIngredientCommand request, CancellationToken cancellationToken)
    {
        var ingredient = new Ingredient
        {
            IngredientName = request.IngredientName.Trim(),
            Quantity = request.Quantity,
            Unit = request.Unit.Trim(),
            ExpirationDate = request.ExpirationDate,
            UnitPrice = request.UnitPrice,
            MinimumQuantity = request.MinimumQuantity
        };

        await _unitOfWork.Ingredients.AddAsync(ingredient, cancellationToken);
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
