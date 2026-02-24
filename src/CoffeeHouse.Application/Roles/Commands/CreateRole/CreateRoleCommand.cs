using CoffeeHouse.Application.Roles.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Roles.Commands.CreateRole;

public record CreateRoleCommand(string Name, string? Description) : IRequest<RoleDto>;
