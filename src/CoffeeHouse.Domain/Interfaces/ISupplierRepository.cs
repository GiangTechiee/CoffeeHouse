using CoffeeHouse.Domain.Entities;

namespace CoffeeHouse.Domain.Interfaces;

public interface ISupplierRepository : IRepository<Supplier>
{
    Task<(IReadOnlyList<Supplier> Items, int TotalCount)> GetPagedAsync(
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
