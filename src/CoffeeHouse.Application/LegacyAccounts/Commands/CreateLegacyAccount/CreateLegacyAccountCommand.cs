using CoffeeHouse.Application.Authentication.DTOs;
using MediatR;

namespace CoffeeHouse.Application.LegacyAccounts.Commands.CreateLegacyAccount;

public record CreateLegacyAccountCommand(
    string Username,
    string Password,
    int RoleId,
    string? Status,
    int? EmployeeId,
    int? CustomerId) : IRequest<AccountInfoDto>;
