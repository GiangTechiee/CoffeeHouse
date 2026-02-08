using CoffeeHouse.Application.Employees.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Employees.Commands.UpdateEmployee;

/// <summary>
/// Command to update an existing employee
/// </summary>
public record UpdateEmployeeCommand : IRequest<EmployeeDto>
{
    /// <summary>
    /// Employee ID to update
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Coffee shop ID where employee works
    /// </summary>
    public int CoffeeShopId { get; init; }

    /// <summary>
    /// Employee full name
    /// </summary>
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// Employee address
    /// </summary>
    public string? Address { get; init; }

    /// <summary>
    /// Employee date of birth
    /// </summary>
    public DateTime? DateOfBirth { get; init; }

    /// <summary>
    /// Employee gender (true = Male, false = Female)
    /// </summary>
    public bool? Gender { get; init; }

    /// <summary>
    /// Employee position/role
    /// </summary>
    public string Position { get; init; } = string.Empty;

    /// <summary>
    /// Employee phone number
    /// </summary>
    public string PhoneNumber { get; init; } = string.Empty;

    /// <summary>
    /// Employee ID card number (CCCD)
    /// </summary>
    public string IdCardNumber { get; init; } = string.Empty;

    /// <summary>
    /// Employee email
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// Base salary
    /// </summary>
    public decimal BaseSalary { get; init; }

    /// <summary>
    /// Salary coefficient
    /// </summary>
    public decimal SalaryCoefficient { get; init; }
}
