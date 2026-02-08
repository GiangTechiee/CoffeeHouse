using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoffeeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementation of IRoleRepository
/// </summary>
public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(CoffeeHouseContext context) : base(context)
    {
    }

    public async Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.RoleName == roleName, cancellationToken);
    }
}
