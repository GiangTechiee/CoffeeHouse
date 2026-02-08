using CoffeeHouse.Domain.Entities;

namespace CoffeeHouse.Domain.Interfaces;

/// <summary>
/// Repository interface for Employee entity with custom queries
/// </summary>
public interface IEmployeeRepository : IRepository<Employee>
{
    /// <summary>
    /// Gets employees by coffee shop ID asynchronously
    /// </summary>
    /// <param name="coffeeShopId">Coffee shop ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Read-only list of employees working at the specified coffee shop</returns>
    Task<IReadOnlyList<Employee>> GetByCoffeeShopAsync(int coffeeShopId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an employee by phone number asynchronously
    /// </summary>
    /// <param name="phoneNumber">Employee phone number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Employee if found, null otherwise</returns>
    Task<Employee?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an employee by ID card number asynchronously
    /// </summary>
    /// <param name="idCardNumber">ID card number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Employee if found, null otherwise</returns>
    Task<Employee?> GetByIdCardNumberAsync(string idCardNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets employees by position asynchronously
    /// </summary>
    /// <param name="position">Employee position</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Read-only list of employees with the specified position</returns>
    Task<IReadOnlyList<Employee>> GetByPositionAsync(string position, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches employees by name asynchronously
    /// </summary>
    /// <param name="searchTerm">Search term to match against employee name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Read-only list of employees matching the search term</returns>
    Task<IReadOnlyList<Employee>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets employees by salary range asynchronously
    /// </summary>
    /// <param name="minSalary">Minimum total salary</param>
    /// <param name="maxSalary">Maximum total salary</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Read-only list of employees within the salary range</returns>
    Task<IReadOnlyList<Employee>> GetBySalaryRangeAsync(decimal minSalary, decimal maxSalary, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an employee with the specified phone number exists
    /// </summary>
    /// <param name="phoneNumber">Phone number</param>
    /// <param name="excludeId">Optional ID to exclude from the check (useful for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if employee exists, false otherwise</returns>
    Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, int? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an employee with the specified ID card number exists
    /// </summary>
    /// <param name="idCardNumber">ID card number</param>
    /// <param name="excludeId">Optional ID to exclude from the check (useful for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if employee exists, false otherwise</returns>
    Task<bool> ExistsByIdCardNumberAsync(string idCardNumber, int? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets employee with their processed orders asynchronously
    /// </summary>
    /// <param name="employeeId">Employee ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Employee with orders if found, null otherwise</returns>
    Task<Employee?> GetWithOrdersAsync(int employeeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets employees using specification pattern with pagination
    /// </summary>
    /// <param name="specification">Specification to apply</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tuple containing list of employees and total count</returns>
    Task<(IReadOnlyList<Employee> Items, int TotalCount)> GetWithSpecificationAsync(
        Func<IQueryable<Employee>, IQueryable<Employee>> specification,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
