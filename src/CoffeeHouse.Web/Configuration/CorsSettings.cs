using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Web.Configuration;

public class CorsSettings
{
    public const string SectionName = "CorsSettings";

    [Required]
    public string AllowedOrigins { get; set; } = string.Empty;
}
