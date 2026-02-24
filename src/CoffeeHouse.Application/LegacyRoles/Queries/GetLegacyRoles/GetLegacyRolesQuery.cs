using CoffeeHouse.Application.LegacyRoles.DTOs;
using MediatR;

namespace CoffeeHouse.Application.LegacyRoles.Queries.GetLegacyRoles;

public record GetLegacyRolesQuery : IRequest<List<LegacyRoleDto>>;
