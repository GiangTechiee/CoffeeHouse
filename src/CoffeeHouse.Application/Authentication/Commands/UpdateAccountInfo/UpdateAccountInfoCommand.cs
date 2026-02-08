using CoffeeHouse.Application.Authentication.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Authentication.Commands.UpdateAccountInfo;

/// <summary>
/// Command to update account and related profile info
/// </summary>
public record UpdateAccountInfoCommand : IRequest<bool>
{
    public string CurrentUsername { get; init; } = string.Empty;
    public string? NewUsername { get; init; }
    public string? NewPassword { get; init; }
    
    // Employee/Customer shared info
    public AccountInfoDto? AccountInfo { get; init; }
}
