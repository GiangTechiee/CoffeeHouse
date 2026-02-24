using CoffeeHouse.Application.Common.Exceptions;
using CoffeeHouse.Application.Roles.DTOs;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CoffeeHouse.Application.Roles.Commands.UpdateRole;

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, RoleDto>
{
    private readonly RoleManager<AppRole> _roleManager;

    public UpdateRoleCommandHandler(RoleManager<AppRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<RoleDto> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(request.Id.ToString());
        if (role == null)
        {
            throw new NotFoundException($"Role with ID {request.Id} not found");
        }

        role.Name = request.Name.Trim();
        role.NormalizedName = request.Name.Trim().ToUpperInvariant();
        role.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();

        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            throw new ApiOperationException("UPDATE_ROLE_FAILED", "Failed to update role", result.Errors);
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
