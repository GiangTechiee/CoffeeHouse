namespace CoffeeHouse.Application.Employees.DTOs;

/// <summary>
/// Lightweight DTO for employee list views (projection)
/// </summary>
public class EmployeeListDto
{
    /// <summary>
    /// Employee ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Employee full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Employee position/role
    /// </summary>
    public string Position { get; set; } = string.Empty;

    /// <summary>
    /// Employee phone number
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Employee email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Calculated total salary
    /// </summary>
    public decimal TotalSalary { get; set; }

    /// <summary>
    /// Coffee shop ID where employee works
    /// </summary>
    public int CoffeeShopId { get; set; }
}
