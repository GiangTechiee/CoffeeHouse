using CoffeeHouse.Application.Authentication.DTOs;
using CoffeeHouse.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using CoffeeHouse.Domain.Identity;
using CoffeeHouse.Application.Interfaces.Security;

namespace CoffeeHouse.Application.Authentication.Commands.Login;

/// <summary>
/// Handler for LoginCommand using Identity and JWT
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthenticationResultDto>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtProvider _jwtProvider;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        UserManager<AppUser> userManager,
        IJwtProvider jwtProvider,
        ILogger<LoginCommandHandler> logger)
    {
        _userManager = userManager;
        _jwtProvider = jwtProvider;
        _logger = logger;
    }

    public async Task<AuthenticationResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Authenticating user: {Username}", request.Username);

        var user = await _userManager.FindByNameAsync(request.Username);

        if (user == null)
        {
            _logger.LogWarning("Authentication failed: User {Username} not found", request.Username);
            return new AuthenticationResultDto { Success = false, Message = "Invalid username or password" };
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!isPasswordValid)
        {
            _logger.LogWarning("Authentication failed: Invalid password for user {Username}", request.Username);
            return new AuthenticationResultDto { Success = false, Message = "Invalid username or password" };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtProvider.GenerateToken(user, roles);

        _logger.LogInformation("User {Username} authenticated successfully", request.Username);

        return new AuthenticationResultDto
        {
            Id = user.Id, // Assuming AppUser Id is int, otherwise AuthenticationResultDto needs update if Id type differs. AppUser inherits IdentityUser<int> so it should be fine.
            Username = user.UserName ?? string.Empty,
            Role = roles.FirstOrDefault() ?? "User",
            FullName = user.FullName,
            Email = user.Email,
            Success = true,
            Message = "Login successful",
            Token = token
        };
    }
}

