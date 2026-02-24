using CoffeeHouse.Application.Roles.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Roles.Queries.GetRoles;

public record GetRolesQuery : IRequest<List<RoleDto>>;
