using CoffeeHouse.Domain.Entities;

namespace CoffeeHouse.Domain.Interfaces;

/// <summary>
/// Repository interface for Product entity with custom queries
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    /// <summary>
    /// Gets products by category ID asynchronously
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Read-only list of products in the specified category</returns>
    Task<IReadOnlyList<Product>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a product by name asynchronously
    /// </summary>
    /// <param name="name">Product name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Product if found, null otherwise</returns>
    Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches products by name or description asynchronously
    /// </summary>
    /// <param name="searchTerm">Search term to match against name or description</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Read-only list of products matching the search term</returns>
    Task<IReadOnlyList<Product>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products within a price range asynchronously
    /// </summary>
    /// <param name="minPrice">Minimum price</param>
    /// <param name="maxPrice">Maximum price</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Read-only list of products within the price range</returns>
    Task<IReadOnlyList<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a product with the specified name exists
    /// </summary>
    /// <param name="name">Product name</param>
    /// <param name="excludeId">Optional ID to exclude from the check (useful for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if product exists, false otherwise</returns>
    Task<bool> ExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paginated list of products with optional filters
    /// </summary>
    /// <param name="categoryId">Optional category ID filter</param>
    /// <param name="searchTerm">Optional search term for name or description</param>
    /// <param name="minPrice">Optional minimum price filter</param>
    /// <param name="maxPrice">Optional maximum price filter</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tuple containing the list of products and total count</returns>
    Task<(IReadOnlyList<Product> Products, int TotalCount)> GetPagedAsync(
        int? categoryId,
        string? searchTerm,
        decimal? minPrice,
        decimal? maxPrice,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
