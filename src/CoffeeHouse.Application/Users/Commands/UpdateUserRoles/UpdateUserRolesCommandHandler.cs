using CoffeeHouse.Application.Common.Exceptions;
using CoffeeHouse.Application.Users.DTOs;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CoffeeHouse.Application.Users.Commands.UpdateUserRoles;

public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand, UserDto>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly ILogger<UpdateUserRolesCommandHandler> _logger;

    public UpdateUserRolesCommandHandler(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        ILogger<UpdateUserRolesCommandHandler> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<UserDto> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            throw new NotFoundException($"User with ID {request.UserId} not found");
        }

        var roles = request.Roles?
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .Select(r => r.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList() ?? new List<string>();

        if (roles.Count > 0)
        {
            var missingRoles = new List<string>();
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    if (request.CreateRolesIfMissing)
                    {
                        var createRole = await _roleManager.CreateAsync(new AppRole
                        {
                            Name = role,
                            NormalizedName = role.ToUpperInvariant()
                        });

                        if (!createRole.Succeeded)
                        {
                            throw new ApiOperationException("CREATE_ROLE_FAILED", "Failed to create role", createRole.Errors);
                        }
                    }
                    else
                    {
                        missingRoles.Add(role);
                    }
                }
            }

            if (missingRoles.Count > 0)
            {
                throw new ApiOperationException("ROLE_NOT_FOUND", "Some roles do not exist", new { missingRoles });
            }
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        var removeRoles = currentRoles.Except(roles).ToList();
        var addRoles = roles.Except(currentRoles).ToList();

        if (removeRoles.Count > 0)
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, removeRoles);
            if (!removeResult.Succeeded)
            {
                throw new ApiOperationException("REMOVE_ROLE_FAILED", "Failed to remove roles", removeResult.Errors);
            }
        }

        if (addRoles.Count > 0)
        {
            var addResult = await _userManager.AddToRolesAsync(user, addRoles);
            if (!addResult.Succeeded)
            {
                throw new ApiOperationException("ASSIGN_ROLE_FAILED", "Failed to assign roles", addResult.Errors);
            }
        }

        var updatedRoles = await _userManager.GetRolesAsync(user);

        _logger.LogInformation("Updated roles for user {UserId}", user.Id);

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
            Roles = updatedRoles.ToList()
        };
    }
}
