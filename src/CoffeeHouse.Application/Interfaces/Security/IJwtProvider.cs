using CoffeeHouse.Domain.Identity;
using System.Security.Claims;

namespace CoffeeHouse.Application.Interfaces.Security;

/// <summary>
/// Interface for JWT token generation and validation
/// </summary>
public interface IJwtProvider
{
    string GenerateToken(AppUser user, IEnumerable<string> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
