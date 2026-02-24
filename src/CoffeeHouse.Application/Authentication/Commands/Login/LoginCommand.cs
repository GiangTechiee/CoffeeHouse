using CoffeeHouse.Application.Authentication.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Authentication.Commands.Login;

/// <summary>
/// Command for authenticating a user
/// </summary>
public record LoginCommand(string Email, string Password) : IRequest<AuthenticationResultDto>;
