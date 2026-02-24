using CoffeeHouse.Application.RefreshTokens.DTOs;
using MediatR;

namespace CoffeeHouse.Application.RefreshTokens.Commands.RevokeRefreshToken;

public record RevokeRefreshTokenCommand(int Id) : IRequest<RefreshTokenDto>;
