using CoffeeHouse.Domain.Entities;

namespace CoffeeHouse.Domain.Interfaces;

public interface IIngredientRepository : IRepository<Ingredient>
{
    Task<(IReadOnlyList<Ingredient> Items, int TotalCount)> GetPagedAsync(
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
