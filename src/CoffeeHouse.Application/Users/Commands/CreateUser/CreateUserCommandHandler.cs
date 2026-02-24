using CoffeeHouse.Application.Common.Exceptions;
using CoffeeHouse.Application.Users.DTOs;
using CoffeeHouse.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CoffeeHouse.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        ILogger<CreateUserCommandHandler> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var userName = string.IsNullOrWhiteSpace(request.UserName) ? request.Email : request.UserName;
        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new ApiOperationException("VALIDATION_ERROR", "Username or Email is required");
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

        var user = new AppUser
        {
            UserName = userName.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            FullName = string.IsNullOrWhiteSpace(request.FullName) ? null : request.FullName.Trim(),
            Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim(),
            EmployeeId = request.EmployeeId,
            CustomerId = request.CustomerId
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            throw new ApiOperationException("CREATE_USER_FAILED", "Failed to create user", createResult.Errors);
        }

        if (roles.Count > 0)
        {
            var addRolesResult = await _userManager.AddToRolesAsync(user, roles);
            if (!addRolesResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                throw new ApiOperationException("ASSIGN_ROLE_FAILED", "Failed to assign roles", addRolesResult.Errors);
            }
        }

        var assignedRoles = await _userManager.GetRolesAsync(user);

        _logger.LogInformation("Created user {UserId} with roles: {Roles}", user.Id, string.Join(", ", assignedRoles));

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
            Roles = assignedRoles.ToList()
        };
    }
}
