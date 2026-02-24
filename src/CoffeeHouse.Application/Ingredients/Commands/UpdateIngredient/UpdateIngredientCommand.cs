using CoffeeHouse.Application.Ingredients.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Ingredients.Commands.UpdateIngredient;

public record UpdateIngredientCommand(
    int IngredientId,
    string IngredientName,
    decimal Quantity,
    string Unit,
    DateTime? ExpirationDate,
    decimal UnitPrice,
    decimal MinimumQuantity) : IRequest<IngredientDto>;
