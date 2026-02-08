using Microsoft.AspNetCore.Identity;

namespace CoffeeHouse.Domain.Identity;

/// <summary>
/// Custom Identity Role entity
/// </summary>
public class AppRole : IdentityRole<int>
{
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
