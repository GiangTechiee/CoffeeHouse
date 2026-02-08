using CoffeeHouse.Application.Customers.DTOs;
using CoffeeHouse.Application.Employees.DTOs;

namespace CoffeeHouse.Application.Authentication.DTOs;

/// <summary>
/// Detailed account information including related employee or customer data
/// </summary>
public class AccountInfoDto
{
    public int AccountId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    
    public EmployeeDto? Employee { get; set; }
    public CustomerDto? Customer { get; set; }
}
