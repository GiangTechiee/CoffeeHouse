using CoffeeHouse.Application.RefreshTokens.DTOs;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.RefreshTokens.Queries.GetRefreshTokenById;

public class GetRefreshTokenByIdQueryHandler : IRequestHandler<GetRefreshTokenByIdQuery, RefreshTokenDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRefreshTokenByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RefreshTokenDto> Handle(GetRefreshTokenByIdQuery request, CancellationToken cancellationToken)
    {
        var token = await _unitOfWork.RefreshTokens.GetByIdAsync(request.Id, cancellationToken);
        if (token == null)
        {
            throw new NotFoundException($"RefreshToken with ID {request.Id} not found");
        }

        return new RefreshTokenDto
        {
            Id = token.Id,
            UserId = token.UserId,
            ExpiresAt = token.ExpiresAt,
            IsRevoked = token.IsRevoked,
            IsUsed = token.IsUsed,
            TokenPreview = MaskToken(token.Token)
        };
    }

    private static string MaskToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return string.Empty;
        }

        var tail = token.Length <= 6 ? token : token[^6..];
        return $"***{tail}";
    }
}
