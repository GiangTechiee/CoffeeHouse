using CoffeeHouse.Application.RefreshTokens.DTOs;
using MediatR;

namespace CoffeeHouse.Application.RefreshTokens.Queries.GetRefreshTokenById;

public record GetRefreshTokenByIdQuery(int Id) : IRequest<RefreshTokenDto>;
