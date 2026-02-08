using CoffeeHouse.Domain.Entities;

namespace CoffeeHouse.Domain.Interfaces;

/// <summary>
/// Repository interface for Customer entity with custom queries
/// </summary>
public interface ICustomerRepository : IRepository<Customer>
{
    /// <summary>
    /// Gets a customer by phone number asynchronously
    /// </summary>
    /// <param name="phoneNumber">Customer phone number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Customer if found, null otherwise</returns>
    Task<Customer?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches customers by name asynchronously
    /// </summary>
    /// <param name="searchTerm">Search term to match against customer name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Read-only list of customers matching the search term</returns>
    Task<IReadOnlyList<Customer>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets customers by address (partial match) asynchronously
    /// </summary>
    /// <param name="address">Address search term</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Read-only list of customers with matching addresses</returns>
    Task<IReadOnlyList<Customer>> GetByAddressAsync(string address, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a customer with the specified phone number exists
    /// </summary>
    /// <param name="phoneNumber">Phone number</param>
    /// <param name="excludeId">Optional ID to exclude from the check (useful for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if customer exists, false otherwise</returns>
    Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, int? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets customers with their order history asynchronously
    /// </summary>
    /// <param name="customerId">Customer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Customer with orders if found, null otherwise</returns>
    Task<Customer?> GetWithOrdersAsync(int customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets top customers by total order amount asynchronously
    /// </summary>
    /// <param name="count">Number of top customers to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Read-only list of top customers</returns>
    Task<IReadOnlyList<Customer>> GetTopCustomersAsync(int count, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paged list of customers with optional search filtering
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Items per page</param>
    /// <param name="searchTerm">Optional search term for filtering</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tuple containing list of items and total count</returns>
    Task<(IReadOnlyList<Customer> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default);
}
