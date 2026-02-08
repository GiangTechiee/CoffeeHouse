using CoffeeHouse.Domain.Entities;

namespace CoffeeHouse.Domain.Interfaces;

/// <summary>
/// Repository interface for Account entity
/// </summary>
public interface IAccountRepository : IRepository<Account>
{
    /// <summary>
    /// Gets an account by username asynchronously
    /// </summary>
    /// <param name="username">Username to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Account if found, null otherwise</returns>
    Task<Account?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a username exists asynchronously
    /// </summary>
    /// <param name="username">Username to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if username exists, false otherwise</returns>
    Task<bool> ExistsAsync(string username, CancellationToken cancellationToken = default);
}
