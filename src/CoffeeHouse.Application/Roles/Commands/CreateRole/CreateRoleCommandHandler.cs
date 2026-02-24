using CoffeeHouse.Application.Common.Exceptions;
using CoffeeHouse.Application.Roles.DTOs;
using CoffeeHouse.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CoffeeHouse.Application.Roles.Commands.CreateRole;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, RoleDto>
{
    private readonly RoleManager<AppRole> _roleManager;

    public CreateRoleCommandHandler(RoleManager<AppRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var exists = await _roleManager.RoleExistsAsync(request.Name);
        if (exists)
        {
            throw new ApiOperationException("ROLE_EXISTS", "Role already exists", new { name = request.Name });
        }

        var role = new AppRole
        {
            Name = request.Name.Trim(),
            NormalizedName = request.Name.Trim().ToUpperInvariant(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim()
        };

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            throw new ApiOperationException("CREATE_ROLE_FAILED", "Failed to create role", result.Errors);
        }

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name ?? string.Empty,
            NormalizedName = role.NormalizedName ?? string.Empty,
            Description = role.Description,
            CreatedAt = role.CreatedAt
        };
    }
}
