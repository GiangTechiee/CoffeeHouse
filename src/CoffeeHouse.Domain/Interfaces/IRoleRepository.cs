using CoffeeHouse.Domain.Entities;

namespace CoffeeHouse.Domain.Interfaces;

/// <summary>
/// Repository interface for Role entity
/// </summary>
public interface IRoleRepository : IRepository<Role>
{
    /// <summary>
    /// Gets a role by name asynchronously
    /// </summary>
    /// <param name="roleName">Role name to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Role if found, null otherwise</returns>
    Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default);
}
