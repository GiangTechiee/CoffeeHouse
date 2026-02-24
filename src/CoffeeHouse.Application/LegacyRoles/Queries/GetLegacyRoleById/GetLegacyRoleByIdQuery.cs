using CoffeeHouse.Application.LegacyRoles.DTOs;
using MediatR;

namespace CoffeeHouse.Application.LegacyRoles.Queries.GetLegacyRoleById;

public record GetLegacyRoleByIdQuery(int RoleId) : IRequest<LegacyRoleDto>;
