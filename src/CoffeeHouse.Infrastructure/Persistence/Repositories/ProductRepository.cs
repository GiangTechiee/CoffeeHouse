using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoffeeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// Product repository implementation with custom queries
/// </summary>
public class ProductRepository : Repository<Product>, IProductRepository
{
    /// <summary>
    /// Initializes a new instance of the ProductRepository class
    /// </summary>
    /// <param name="context">Database context</param>
    public ProductRepository(DbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets products by category ID asynchronously
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Read-only list of products in the specified category</returns>
    public async Task<IReadOnlyList<Product>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .OrderBy(p => p.ProductName)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a product by name asynchronously
    /// </summary>
    /// <param name="name">Product name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Product if found, null otherwise</returns>
    public async Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProductName == name, cancellationToken);
    }

    /// <summary>
    /// Searches products by name or description asynchronously
    /// </summary>
    /// <param name="searchTerm">Search term to match against name or description</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Read-only list of products matching the search term</returns>
    public async Task<IReadOnlyList<Product>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllAsync(cancellationToken);
        }

        var lowerSearchTerm = searchTerm.ToLower();

        return await _dbSet
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => 
                p.ProductName.ToLower().Contains(lowerSearchTerm) || 
                (p.Description != null && p.Description.ToLower().Contains(lowerSearchTerm)))
            .OrderBy(p => p.ProductName)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets products within a price range asynchronously
    /// </summary>
    /// <param name="minPrice">Minimum price</param>
    /// <param name="maxPrice">Maximum price</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Read-only list of products within the price range</returns>
    public async Task<IReadOnlyList<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
            .OrderBy(p => p.Price)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Checks if a product with the specified name exists
    /// </summary>
    /// <param name="name">Product name</param>
    /// <param name="excludeId">Optional ID to exclude from the check (useful for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if product exists, false otherwise</returns>
    public async Task<bool> ExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        var query = _dbSet.AsNoTracking().Where(p => p.ProductName == name);

        if (excludeId.HasValue)
        {
            query = query.Where(p => p.ProductId != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

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
    public async Task<(IReadOnlyList<Product> Products, int TotalCount)> GetPagedAsync(
        int? categoryId,
        string? searchTerm,
        decimal? minPrice,
        decimal? maxPrice,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        // Start with base query
        IQueryable<Product> query = _dbSet.AsNoTracking().Include(p => p.Category);

        // Apply category filter
        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        // Apply search term filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(p => 
                p.ProductName.ToLower().Contains(lowerSearchTerm) || 
                (p.Description != null && p.Description.ToLower().Contains(lowerSearchTerm)));
        }

        // Apply price range filters
        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination and ordering
        var products = await query
            .OrderBy(p => p.ProductName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (products, totalCount);
    }
}
