using System.Linq.Expressions;
using CoffeeHouse.Domain.Entities;

namespace CoffeeHouse.Application.Employees.Specifications;

/// <summary>
/// Specification pattern for Employee queries
/// </summary>
public class EmployeeSpecification
{
    private readonly List<Expression<Func<Employee, bool>>> _criteria = new();
    private Expression<Func<Employee, object>>? _orderBy;
    private bool _orderByDescending;

    /// <summary>
    /// Gets the list of criteria expressions
    /// </summary>
    public IReadOnlyList<Expression<Func<Employee, bool>>> Criteria => _criteria.AsReadOnly();

    /// <summary>
    /// Gets the order by expression
    /// </summary>
    public Expression<Func<Employee, object>>? OrderBy => _orderBy;

    /// <summary>
    /// Gets whether to order descending
    /// </summary>
    public bool OrderByDescending => _orderByDescending;

    /// <summary>
    /// Adds a criteria to filter by coffee shop
    /// </summary>
    /// <param name="coffeeShopId">Coffee shop ID</param>
    /// <returns>This specification for chaining</returns>
    public EmployeeSpecification ByCoffeeShop(int coffeeShopId)
    {
        _criteria.Add(e => e.StoreId == coffeeShopId);
        return this;
    }

    /// <summary>
    /// Adds a criteria to filter by position
    /// </summary>
    /// <param name="position">Position</param>
    /// <returns>This specification for chaining</returns>
    public EmployeeSpecification ByPosition(string position)
    {
        if (!string.IsNullOrWhiteSpace(position))
        {
            _criteria.Add(e => e.Position == position);
        }
        return this;
    }

    /// <summary>
    /// Adds a criteria to search by name or phone number
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <returns>This specification for chaining</returns>
    public EmployeeSpecification SearchByNameOrPhone(string searchTerm)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            _criteria.Add(e => 
                e.FullName.ToLower().Contains(lowerSearchTerm) || 
                e.PhoneNumber.Contains(searchTerm));
        }
        return this;
    }

    /// <summary>
    /// Adds a criteria to filter by salary range
    /// </summary>
    /// <param name="minSalary">Minimum salary</param>
    /// <param name="maxSalary">Maximum salary</param>
    /// <returns>This specification for chaining</returns>
    public EmployeeSpecification BySalaryRange(decimal? minSalary, decimal? maxSalary)
    {
        if (minSalary.HasValue)
        {
            _criteria.Add(e => e.BaseSalary * e.SalaryCoefficient >= minSalary.Value);
        }

        if (maxSalary.HasValue)
        {
            _criteria.Add(e => e.BaseSalary * e.SalaryCoefficient <= maxSalary.Value);
        }

        return this;
    }

    /// <summary>
    /// Sets ordering by full name
    /// </summary>
    /// <param name="descending">Whether to order descending</param>
    /// <returns>This specification for chaining</returns>
    public EmployeeSpecification OrderByFullName(bool descending = false)
    {
        _orderBy = e => e.FullName;
        _orderByDescending = descending;
        return this;
    }

    /// <summary>
    /// Sets ordering by salary
    /// </summary>
    /// <param name="descending">Whether to order descending</param>
    /// <returns>This specification for chaining</returns>
    public EmployeeSpecification OrderBySalary(bool descending = false)
    {
        _orderBy = e => e.BaseSalary * e.SalaryCoefficient;
        _orderByDescending = descending;
        return this;
    }

    /// <summary>
    /// Applies the specification to a queryable
    /// </summary>
    /// <param name="query">The queryable to apply to</param>
    /// <returns>The filtered and ordered queryable</returns>
    public IQueryable<Employee> Apply(IQueryable<Employee> query)
    {
        // Apply all criteria
        foreach (var criterion in _criteria)
        {
            query = query.Where(criterion);
        }

        // Apply ordering
        if (_orderBy != null)
        {
            query = _orderByDescending 
                ? query.OrderByDescending(_orderBy) 
                : query.OrderBy(_orderBy);
        }

        return query;
    }
}
