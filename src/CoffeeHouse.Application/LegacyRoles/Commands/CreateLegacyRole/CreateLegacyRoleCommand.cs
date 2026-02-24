using CoffeeHouse.Application.LegacyRoles.DTOs;
using MediatR;

namespace CoffeeHouse.Application.LegacyRoles.Commands.CreateLegacyRole;

public record CreateLegacyRoleCommand(string Name, string? Description) : IRequest<LegacyRoleDto>;
