using CoffeeHouse.Application.Ingredients.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Ingredients.Commands.CreateIngredient;

public record CreateIngredientCommand(
    string IngredientName,
    decimal Quantity,
    string Unit,
    DateTime? ExpirationDate,
    decimal UnitPrice,
    decimal MinimumQuantity) : IRequest<IngredientDto>;
