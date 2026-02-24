using CoffeeHouse.Application.Authentication.DTOs;
using MediatR;

namespace CoffeeHouse.Application.LegacyAccounts.Commands.UpdateLegacyAccount;

public record UpdateLegacyAccountCommand(
    int AccountId,
    string Username,
    string? Password,
    int RoleId,
    string? Status,
    int? EmployeeId,
    int? CustomerId) : IRequest<AccountInfoDto>;
