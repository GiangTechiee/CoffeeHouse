using CoffeeHouse.Application.Users.DTOs;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Application.Users.Commands.CreateUser;

public record CreateUserCommand : IRequest<UserDto>
{
    public string? UserName { get; init; }

    [EmailAddress]
    public string? Email { get; init; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; init; } = string.Empty;

    public string? FullName { get; init; }
    public string? Address { get; init; }
    public int? EmployeeId { get; init; }
    public int? CustomerId { get; init; }
    public List<string>? Roles { get; init; }
    public bool CreateRolesIfMissing { get; init; } = false;
}
