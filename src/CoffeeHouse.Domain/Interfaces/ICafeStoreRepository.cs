using CoffeeHouse.Domain.Entities;

namespace CoffeeHouse.Domain.Interfaces;

public interface ICafeStoreRepository : IRepository<CafeStore>
{
    Task<(IReadOnlyList<CafeStore> Items, int TotalCount)> GetPagedAsync(
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
