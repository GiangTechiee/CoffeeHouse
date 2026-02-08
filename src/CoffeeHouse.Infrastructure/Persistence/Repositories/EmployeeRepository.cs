using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoffeeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// Employee repository implementation with custom queries
/// </summary>
public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    /// <summary>
    /// Initializes a new instance of the EmployeeRepository class
    /// </summary>
    /// <param name="context">Database context</param>
    public EmployeeRepository(DbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets employees by coffee shop ID asynchronously
    /// </summary>
    /// <param name="coffeeShopId">Coffee shop ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Read-only list of employees working at the specified coffee shop</returns>
    public async Task<IReadOnlyList<Employee>> GetByCoffeeShopAsync(int coffeeShopId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(e => e.StoreId == coffeeShopId)
            .OrderBy(e => e.FullName)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets an employee by phone number asynchronously
    /// </summary>
    /// <param name="phoneNumber">Employee phone number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Employee if found, null otherwise</returns>
    public async Task<Employee?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return null;
        }

        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.PhoneNumber == phoneNumber, cancellationToken);
    }

    /// <summary>
    /// Gets an employee by ID card number asynchronously
    /// </summary>
    /// <param name="idCardNumber">ID card number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Employee if found, null otherwise</returns>
    public async Task<Employee?> GetByIdCardNumberAsync(string idCardNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(idCardNumber))
        {
            return null;
        }

        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.IdentityCardNumber == idCardNumber, cancellationToken);
    }

    /// <summary>
    /// Gets employees by position asynchronously
    /// </summary>
    /// <param name="position">Employee position</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Read-only list of employees with the specified position</returns>
    public async Task<IReadOnlyList<Employee>> GetByPositionAsync(string position, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(position))
        {
            return new List<Employee>();
        }

        return await _dbSet
            .AsNoTracking()
            .Where(e => e.Position == position)
            .OrderBy(e => e.FullName)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Searches employees by name asynchronously
    /// </summary>
    /// <param name="searchTerm">Search term to match against employee name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Read-only list of employees matching the search term</returns>
    public async Task<IReadOnlyList<Employee>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllAsync(cancellationToken);
        }

        var lowerSearchTerm = searchTerm.ToLower();

        return await _dbSet
            .AsNoTracking()
            .Where(e => e.FullName.ToLower().Contains(lowerSearchTerm))
            .OrderBy(e => e.FullName)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets employees by salary range asynchronously
    /// </summary>
    /// <param name="minSalary">Minimum total salary</param>
    /// <param name="maxSalary">Maximum total salary</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Read-only list of employees within the salary range</returns>
    public async Task<IReadOnlyList<Employee>> GetBySalaryRangeAsync(decimal minSalary, decimal maxSalary, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(e => e.BaseSalary * e.SalaryCoefficient >= minSalary && 
                       e.BaseSalary * e.SalaryCoefficient <= maxSalary)
            .OrderBy(e => e.BaseSalary * e.SalaryCoefficient)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Checks if an employee with the specified phone number exists
    /// </summary>
    /// <param name="phoneNumber">Phone number</param>
    /// <param name="excludeId">Optional ID to exclude from the check (useful for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if employee exists, false otherwise</returns>
    public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return false;
        }

        var query = _dbSet.AsNoTracking().Where(e => e.PhoneNumber == phoneNumber);

        if (excludeId.HasValue)
        {
            query = query.Where(e => e.EmployeeId != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Checks if an employee with the specified ID card number exists
    /// </summary>
    /// <param name="idCardNumber">ID card number</param>
    /// <param name="excludeId">Optional ID to exclude from the check (useful for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if employee exists, false otherwise</returns>
    public async Task<bool> ExistsByIdCardNumberAsync(string idCardNumber, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(idCardNumber))
        {
            return false;
        }

        var query = _dbSet.AsNoTracking().Where(e => e.IdentityCardNumber == idCardNumber);

        if (excludeId.HasValue)
        {
            query = query.Where(e => e.EmployeeId != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Gets employee with their processed orders asynchronously
    /// </summary>
    /// <param name="employeeId">Employee ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Employee with orders if found, null otherwise</returns>
    public async Task<Employee?> GetWithOrdersAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        // Note: This assumes there's a navigation property from Employee to Orders
        // If not configured in EF Core, this will need to be adjusted
        // For now, we'll return the employee without orders since the domain entity doesn't have this navigation
        return await GetByIdAsync(employeeId, cancellationToken);
    }

    /// <summary>
    /// Gets employees using specification pattern with pagination
    /// </summary>
    /// <param name="specification">Specification to apply</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tuple containing list of employees and total count</returns>
    public async Task<(IReadOnlyList<Employee> Items, int TotalCount)> GetWithSpecificationAsync(
        Func<IQueryable<Employee>, IQueryable<Employee>> specification,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        // Start with base query
        var query = _dbSet.AsNoTracking();

        // Apply specification
        query = specification(query);

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
