using MediatR;
using CoffeeHouse.Application.Authentication.DTOs;

namespace CoffeeHouse.Application.Authentication.Commands.AuthenticateUser;

/// <summary>
/// Command to authenticate a user
/// </summary>
public record AuthenticateUserCommand(string Username, string Password) : IRequest<AuthResultDto>;
