using CoffeeHouse.Application.Roles.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Roles.Commands.UpdateRole;

public record UpdateRoleCommand(int Id, string Name, string? Description) : IRequest<RoleDto>;
