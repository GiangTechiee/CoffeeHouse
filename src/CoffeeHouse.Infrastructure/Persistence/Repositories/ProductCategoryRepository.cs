using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoffeeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// ProductCategory repository implementation
/// </summary>
public class ProductCategoryRepository : Repository<ProductCategory>, IProductCategoryRepository
{
    public ProductCategoryRepository(DbContext context) : base(context)
    {
    }

    public async Task<ProductCategory?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(pc => pc.CategoryName == name, cancellationToken);
    }
}
