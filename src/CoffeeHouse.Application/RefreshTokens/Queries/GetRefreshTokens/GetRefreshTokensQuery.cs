using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.RefreshTokens.DTOs;
using MediatR;

namespace CoffeeHouse.Application.RefreshTokens.Queries.GetRefreshTokens;

public record GetRefreshTokensQuery(
    int PageNumber = 1,
    int PageSize = 20,
    int? UserId = null,
    bool? ActiveOnly = null) : IRequest<PagedResult<RefreshTokenDto>>;
