using CoffeeHouse.Application.Authentication.DTOs;
using CoffeeHouse.Application.Interfaces.Security;
using CoffeeHouse.Domain.Identity;
using CoffeeHouse.Domain.Interfaces;
using CoffeeHouse.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CoffeeHouse.Application.Authentication.Commands.AuthenticateUser;

public class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, AuthResultDto>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtProvider _jwtProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AuthenticateUserCommandHandler> _logger;

    public AuthenticateUserCommandHandler(
        UserManager<AppUser> userManager,
        IJwtProvider jwtProvider,
        IUnitOfWork unitOfWork,
        ILogger<AuthenticateUserCommandHandler> logger)
    {
        _userManager = userManager;
        _jwtProvider = jwtProvider;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AuthResultDto> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to authenticate user: {Username}", request.Username);

        var user = await _userManager.FindByNameAsync(request.Username);

        if (user == null)
        {
            _logger.LogWarning("Authentication failed: User {Username} not found", request.Username);
            return new AuthResultDto { Success = false, Message = "Invalid credentials" };
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!isPasswordValid)
        {
            _logger.LogWarning("Authentication failed: Invalid password for user {Username}", request.Username);
            return new AuthResultDto { Success = false, Message = "Invalid credentials" };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtProvider.GenerateToken(user, roles);
        var refreshToken = _jwtProvider.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7), // Configurable
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {Username} authenticated successfully", request.Username);

        return new AuthResultDto
        {
            Success = true,
            Username = user.UserName ?? string.Empty,
            Role = roles.FirstOrDefault() ?? "User",
            EmployeeId = user.EmployeeId,
            CustomerId = user.CustomerId,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}
