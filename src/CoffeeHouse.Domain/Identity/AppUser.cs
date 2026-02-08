using Microsoft.AspNetCore.Identity;

namespace CoffeeHouse.Domain.Identity;

/// <summary>
/// Custom Identity User entity
/// </summary>
public class AppUser : IdentityUser<int>
{
    public string? FullName { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    
    // Links to existing domain entities
    public int? EmployeeId { get; set; }
    public int? CustomerId { get; set; }
}
