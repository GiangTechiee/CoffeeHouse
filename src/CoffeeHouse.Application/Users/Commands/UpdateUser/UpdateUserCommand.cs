using CoffeeHouse.Application.Users.DTOs;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand : IRequest<UserDto>
{
    [Required]
    public int Id { get; init; }

    public string? UserName { get; init; }

    [EmailAddress]
    public string? Email { get; init; }

    public string? FullName { get; init; }
    public string? Address { get; init; }
    public int? EmployeeId { get; init; }
    public int? CustomerId { get; init; }
    public bool? LockoutEnabled { get; init; }
    public DateTimeOffset? LockoutEnd { get; init; }

    [StringLength(100, MinimumLength = 6)]
    public string? NewPassword { get; init; }
}
