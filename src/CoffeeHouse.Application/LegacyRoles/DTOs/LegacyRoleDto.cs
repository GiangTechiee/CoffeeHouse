namespace CoffeeHouse.Application.LegacyRoles.DTOs;

public class LegacyRoleDto
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
}
