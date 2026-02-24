namespace CoffeeHouse.Application.RefreshTokens.DTOs;

public class RefreshTokenDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public bool IsUsed { get; set; }
    public string TokenPreview { get; set; } = string.Empty;
}
