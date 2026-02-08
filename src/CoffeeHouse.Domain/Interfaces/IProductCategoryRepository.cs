using CoffeeHouse.Domain.Entities;

namespace CoffeeHouse.Domain.Interfaces;

/// <summary>
/// Repository interface for ProductCategory entity
/// </summary>
public interface IProductCategoryRepository : IRepository<ProductCategory>
{
    /// <summary>
    /// Gets a category by name asynchronously
    /// </summary>
    /// <param name="name">Category name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ProductCategory if found, null otherwise</returns>
    Task<ProductCategory?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
