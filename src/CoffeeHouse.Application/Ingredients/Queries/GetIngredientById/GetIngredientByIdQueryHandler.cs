using CoffeeHouse.Application.Ingredients.DTOs;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.Ingredients.Queries.GetIngredientById;

public class GetIngredientByIdQueryHandler : IRequestHandler<GetIngredientByIdQuery, IngredientDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetIngredientByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IngredientDto> Handle(GetIngredientByIdQuery request, CancellationToken cancellationToken)
    {
        var ingredient = await _unitOfWork.Ingredients.GetByIdAsync(request.IngredientId, cancellationToken);
        if (ingredient == null)
        {
            throw new NotFoundException($"Ingredient with ID {request.IngredientId} not found");
        }

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
