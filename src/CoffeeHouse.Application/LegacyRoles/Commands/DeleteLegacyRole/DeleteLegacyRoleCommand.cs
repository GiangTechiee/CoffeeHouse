using MediatR;

namespace CoffeeHouse.Application.LegacyRoles.Commands.DeleteLegacyRole;

public record DeleteLegacyRoleCommand(int RoleId) : IRequest;
