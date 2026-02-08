using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoffeeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// Account repository implementation
/// </summary>
public class AccountRepository : Repository<Account>, IAccountRepository
{
    public AccountRepository(DbContext context) : base(context)
    {
    }

    public async Task<Account?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Role)
            .Include(a => a.Employee)
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.Username == username, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(a => a.Username == username, cancellationToken);
    }
}
