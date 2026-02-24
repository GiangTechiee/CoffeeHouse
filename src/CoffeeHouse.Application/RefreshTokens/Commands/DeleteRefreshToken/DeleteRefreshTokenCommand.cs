using MediatR;

namespace CoffeeHouse.Application.RefreshTokens.Commands.DeleteRefreshToken;

public record DeleteRefreshTokenCommand(int Id) : IRequest;
