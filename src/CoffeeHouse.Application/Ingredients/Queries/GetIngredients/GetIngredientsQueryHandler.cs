using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Ingredients.DTOs;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.Ingredients.Queries.GetIngredients;

public class GetIngredientsQueryHandler : IRequestHandler<GetIngredientsQuery, PagedResult<IngredientDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetIngredientsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<IngredientDto>> Handle(GetIngredientsQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : request.PageSize;
        if (pageSize > 100) pageSize = 100;

        var (items, totalCount) = await _unitOfWork.Ingredients.GetPagedAsync(
            request.SearchTerm,
            pageNumber,
            pageSize,
            cancellationToken);

        var dtos = items.Select(MapToDto).ToList();

        return new PagedResult<IngredientDto>
        {
            Items = dtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    private static IngredientDto MapToDto(Domain.Entities.Ingredient ingredient)
    {
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
