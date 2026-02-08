using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Employees.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Employees.Queries.GetEmployees;

/// <summary>
/// Query to get a paginated list of employees
/// </summary>
public record GetEmployeesQuery : IRequest<PagedResult<EmployeeDto>>
{
    /// <summary>
    /// Optional coffee shop ID to filter employees
    /// </summary>
    public int? CoffeeShopId { get; init; }

    /// <summary>
    /// Optional search term to filter employees by name or phone number
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Optional position to filter employees
    /// </summary>
    public string? Position { get; init; }

    /// <summary>
    /// Optional minimum salary filter
    /// </summary>
    public decimal? MinSalary { get; init; }

    /// <summary>
    /// Optional maximum salary filter
    /// </summary>
    public decimal? MaxSalary { get; init; }

    /// <summary>
    /// Page number (1-based). Default is 1.
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Number of items per page. Default is 30.
    /// </summary>
    public int PageSize { get; init; } = 30;
}
