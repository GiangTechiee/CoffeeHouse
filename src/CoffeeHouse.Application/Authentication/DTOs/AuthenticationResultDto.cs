namespace CoffeeHouse.Application.Authentication.DTOs;

/// <summary>
/// Result of an authentication request
/// </summary>
public class AuthenticationResultDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? Token { get; set; }
}
