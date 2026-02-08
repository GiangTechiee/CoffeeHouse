using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Web.Configuration;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    [Required]
    [MinLength(32, ErrorMessage = "JWT SecretKey must be at least 32 characters long")]
    public string SecretKey { get; set; } = string.Empty;

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Range(1, 1440, ErrorMessage = "ExpiryMinutes must be between 1 and 1440")]
    public int ExpiryMinutes { get; set; }
}
