using CoffeeHouse.Application.Ingredients.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Ingredients.Queries.GetIngredientById;

public record GetIngredientByIdQuery(int IngredientId) : IRequest<IngredientDto>;
