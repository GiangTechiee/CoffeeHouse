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
        _logger.LogInformation("Authenticating user: {Email}", request.Email);

        var user = await _userManager.FindByNameAsync(request.Email);

        if (user == null)
        {
            _logger.LogWarning("Authentication failed: User {Email} not found", request.Email);
            return new AuthenticationResultDto { Success = false, Message = "Email hoặc mật khẩu không chính xác" };
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!isPasswordValid)
        {
            _logger.LogWarning("Authentication failed: Invalid password for user {Email}", request.Email);
            return new AuthenticationResultDto { Success = false, Message = "Email hoặc mật khẩu không chính xác" };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtProvider.GenerateToken(user, roles);

        _logger.LogInformation("User {Email} authenticated successfully", request.Email);

        return new AuthenticationResultDto
        {
            Id = user.Id, // Assuming AppUser Id is int, otherwise AuthenticationResultDto needs update if Id type differs. AppUser inherits IdentityUser<int> so it should be fine.
            Email = user.Email ?? user.UserName ?? string.Empty,
            Role = roles.FirstOrDefault() ?? "User",
            FullName = user.FullName,
            Success = true,
            Message = "Login successful",
            Token = token
        };
    }
}

