using CoffeeHouse.Application.Authentication.DTOs;
using CoffeeHouse.Application.Common.Models;
using MediatR;

namespace CoffeeHouse.Application.LegacyAccounts.Queries.GetLegacyAccounts;

public record GetLegacyAccountsQuery(int PageNumber = 1, int PageSize = 20, string? SearchTerm = null)
    : IRequest<PagedResult<AccountInfoDto>>;
