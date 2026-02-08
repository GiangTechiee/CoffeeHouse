namespace CoffeeHouse.Domain.Entities;

/// <summary>
/// Entity for managing JWT refresh tokens
/// </summary>
public class RefreshToken : BaseEntity
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public bool IsUsed { get; set; }
    
    public int UserId { get; set; }
    // We don't use AppUser here to keep Domain/Entities clean of Identity dependency if possible, 
    // but in this project they are mixed. Let's keep it simple.
}
