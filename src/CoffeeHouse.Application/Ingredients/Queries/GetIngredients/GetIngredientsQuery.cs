using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Ingredients.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Ingredients.Queries.GetIngredients;

public record GetIngredientsQuery(int PageNumber = 1, int PageSize = 20, string? SearchTerm = null)
    : IRequest<PagedResult<IngredientDto>>;
