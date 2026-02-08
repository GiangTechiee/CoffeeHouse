using CoffeeHouse.Application.Employees.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Employees.Queries.GetEmployeeById;

/// <summary>
/// Query to get an employee by ID
/// </summary>
public record GetEmployeeByIdQuery : IRequest<EmployeeDto?>
{
    /// <summary>
    /// Employee ID
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Whether to include related orders
    /// </summary>
    public bool IncludeOrders { get; init; }
}
