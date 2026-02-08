using CoffeeHouse.Application.Employees.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Employees.Commands.CreateEmployee;

/// <summary>
/// Command to create a new employee
/// </summary>
public record CreateEmployeeCommand : IRequest<EmployeeDto>
{
    /// <summary>
    /// Coffee shop ID where employee will work
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

    /// <summary>
    /// Account username (optional - for creating employee account)
    /// </summary>
    public string? AccountUsername { get; init; }

    /// <summary>
    /// Account password (optional - for creating employee account)
    /// </summary>
    public string? AccountPassword { get; init; }

    /// <summary>
    /// Account role ID (optional - for creating employee account)
    /// </summary>
    public int? AccountRoleId { get; init; }
}
