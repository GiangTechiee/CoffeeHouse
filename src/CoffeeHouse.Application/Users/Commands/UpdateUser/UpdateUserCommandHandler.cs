using CoffeeHouse.Application.Common.Exceptions;
using CoffeeHouse.Application.Users.DTOs;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CoffeeHouse.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    public UpdateUserCommandHandler(UserManager<AppUser> userManager, ILogger<UpdateUserCommandHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user == null)
        {
            throw new NotFoundException($"User with ID {request.Id} not found");
        }

        if (!string.IsNullOrWhiteSpace(request.UserName))
        {
            user.UserName = request.UserName.Trim();
        }

        if (request.Email != null)
        {
            user.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        }

        if (request.FullName != null)
        {
            user.FullName = string.IsNullOrWhiteSpace(request.FullName) ? null : request.FullName.Trim();
        }

        if (request.Address != null)
        {
            user.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();
        }

        user.EmployeeId = request.EmployeeId;
        user.CustomerId = request.CustomerId;

        if (request.LockoutEnabled.HasValue)
        {
            user.LockoutEnabled = request.LockoutEnabled.Value;
        }

        if (request.LockoutEnd.HasValue)
        {
            user.LockoutEnd = request.LockoutEnd;
        }

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            throw new ApiOperationException("UPDATE_USER_FAILED", "Failed to update user", updateResult.Errors);
        }

        if (!string.IsNullOrWhiteSpace(request.NewPassword))
        {
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);
            if (!resetResult.Succeeded)
            {
                throw new ApiOperationException("RESET_PASSWORD_FAILED", "Failed to reset password", resetResult.Errors);
            }
        }

        var roles = await _userManager.GetRolesAsync(user);

        _logger.LogInformation("Updated user {UserId}", user.Id);

        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email,
            FullName = user.FullName,
            Address = user.Address,
            EmployeeId = user.EmployeeId,
            CustomerId = user.CustomerId,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            Roles = roles.ToList()
        };
    }
}
