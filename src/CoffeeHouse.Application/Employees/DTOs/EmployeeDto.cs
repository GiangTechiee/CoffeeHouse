namespace CoffeeHouse.Application.Employees.DTOs;

/// <summary>
/// Data Transfer Object for Employee
/// </summary>
public class EmployeeDto
{
    /// <summary>
    /// Employee ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Coffee shop ID where employee works
    /// </summary>
    public int CoffeeShopId { get; set; }

    /// <summary>
    /// Employee full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Employee address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Employee date of birth
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Employee gender (true = Male, false = Female)
    /// </summary>
    public bool? Gender { get; set; }

    /// <summary>
    /// Employee position/role
    /// </summary>
    public string Position { get; set; } = string.Empty;

    /// <summary>
    /// Employee phone number
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Employee ID card number (CCCD)
    /// </summary>
    public string IdCardNumber { get; set; } = string.Empty;

    /// <summary>
    /// Employee email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Base salary
    /// </summary>
    public decimal BaseSalary { get; set; }

    /// <summary>
    /// Salary coefficient
    /// </summary>
    public decimal SalaryCoefficient { get; set; }

    /// <summary>
    /// Calculated total salary
    /// </summary>
    public decimal TotalSalary { get; set; }

    /// <summary>
    /// Date when the employee was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date when the employee was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
