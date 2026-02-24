using CoffeeHouse.Application.Authentication.DTOs;
using MediatR;

namespace CoffeeHouse.Application.LegacyAccounts.Queries.GetLegacyAccountById;

public record GetLegacyAccountByIdQuery(int AccountId) : IRequest<AccountInfoDto>;
