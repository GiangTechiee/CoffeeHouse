namespace CoffeeHouse.Application.Authentication.DTOs;

public class AuthResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int? EmployeeId { get; set; }
    public int? CustomerId { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}
