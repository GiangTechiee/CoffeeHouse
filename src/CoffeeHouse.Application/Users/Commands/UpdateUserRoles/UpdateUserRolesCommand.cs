using CoffeeHouse.Application.Users.DTOs;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Application.Users.Commands.UpdateUserRoles;

public record UpdateUserRolesCommand : IRequest<UserDto>
{
    [Required]
    public int UserId { get; init; }

    public List<string>? Roles { get; init; }
    public bool CreateRolesIfMissing { get; init; } = false;
}
