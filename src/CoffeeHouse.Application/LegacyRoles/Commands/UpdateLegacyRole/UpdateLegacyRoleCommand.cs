using CoffeeHouse.Application.LegacyRoles.DTOs;
using MediatR;

namespace CoffeeHouse.Application.LegacyRoles.Commands.UpdateLegacyRole;

public record UpdateLegacyRoleCommand(int RoleId, string Name, string? Description) : IRequest<LegacyRoleDto>;
