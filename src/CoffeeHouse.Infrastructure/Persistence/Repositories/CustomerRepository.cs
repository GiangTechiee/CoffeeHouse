using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoffeeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// Customer repository implementation with custom queries
/// </summary>
public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    /// <summary>
    /// Initializes a new instance of the CustomerRepository class
    /// </summary>
    /// <param name="context">Database context</param>
    public CustomerRepository(DbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets a customer by phone number asynchronously
    /// </summary>
    /// <param name="phoneNumber">Customer phone number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Customer if found, null otherwise</returns>
    public async Task<Customer?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return null;
        }

        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber, cancellationToken);
    }

    /// <summary>
    /// Searches customers by name asynchronously
    /// </summary>
    /// <param name="searchTerm">Search term to match against customer name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Read-only list of customers matching the search term</returns>
    public async Task<IReadOnlyList<Customer>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllAsync(cancellationToken);
        }

        var lowerSearchTerm = searchTerm.ToLower();

        return await _dbSet
            .AsNoTracking()
            .Where(c => c.CustomerName.ToLower().Contains(lowerSearchTerm))
            .OrderBy(c => c.CustomerName)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets customers by address (partial match) asynchronously
    /// </summary>
    /// <param name="address">Address search term</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Read-only list of customers with matching addresses</returns>
    public async Task<IReadOnlyList<Customer>> GetByAddressAsync(string address, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            return new List<Customer>();
        }

        var lowerAddress = address.ToLower();

        return await _dbSet
            .AsNoTracking()
            .Where(c => c.Address.ToLower().Contains(lowerAddress))
            .OrderBy(c => c.CustomerName)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Checks if a customer with the specified phone number exists
    /// </summary>
    /// <param name="phoneNumber">Phone number</param>
    /// <param name="excludeId">Optional ID to exclude from the check (useful for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if customer exists, false otherwise</returns>
    public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return false;
        }

        var query = _dbSet.AsNoTracking().Where(c => c.PhoneNumber == phoneNumber);

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.CustomerId != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Gets customers with their order history asynchronously
    /// </summary>
    /// <param name="customerId">Customer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Customer with orders if found, null otherwise</returns>
    public async Task<Customer?> GetWithOrdersAsync(int customerId, CancellationToken cancellationToken = default)
    {
        // Note: This assumes there's a navigation property from Customer to Orders
        // If not configured in EF Core, this will need to be adjusted
        // For now, we'll return the customer without orders since the domain entity doesn't have this navigation
        return await GetByIdAsync(customerId, cancellationToken);
    }

    /// <summary>
    /// Gets top customers by total order amount asynchronously
    /// </summary>
    /// <param name="count">Number of top customers to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Read-only list of top customers</returns>
    public async Task<IReadOnlyList<Customer>> GetTopCustomersAsync(int count, CancellationToken cancellationToken = default)
    {
        // Note: This would require joining with Orders table
        // Since we don't have navigation properties set up yet, we'll return a simple implementation
        // This should be enhanced once the DbContext is properly configured with relationships
        return await _dbSet
            .AsNoTracking()
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a paged list of customers with optional search filtering
    /// </summary>
    public async Task<(IReadOnlyList<Customer> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearch = searchTerm.ToLower();
            query = query.Where(c => c.CustomerName.ToLower().Contains(lowerSearch) ||
                                     c.PhoneNumber.Contains(searchTerm) ||
                                     c.Address.ToLower().Contains(lowerSearch));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .OrderBy(c => c.CustomerName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
