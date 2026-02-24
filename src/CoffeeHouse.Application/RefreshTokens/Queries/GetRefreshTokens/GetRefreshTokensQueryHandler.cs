using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.RefreshTokens.DTOs;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.RefreshTokens.Queries.GetRefreshTokens;

public class GetRefreshTokensQueryHandler : IRequestHandler<GetRefreshTokensQuery, PagedResult<RefreshTokenDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRefreshTokensQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<RefreshTokenDto>> Handle(GetRefreshTokensQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : request.PageSize;
        if (pageSize > 100) pageSize = 100;

        var allTokens = await _unitOfWork.RefreshTokens.GetAllAsync(cancellationToken);

        var query = allTokens.AsQueryable();
        if (request.UserId.HasValue)
        {
            query = query.Where(t => t.UserId == request.UserId.Value);
        }

        if (request.ActiveOnly.HasValue)
        {
            if (request.ActiveOnly.Value)
            {
                query = query.Where(t => !t.IsRevoked && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow);
            }
            else
            {
                query = query.Where(t => t.IsRevoked || t.IsUsed || t.ExpiresAt <= DateTime.UtcNow);
            }
        }

        var totalCount = query.Count();
        var items = query
            .OrderByDescending(t => t.ExpiresAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new RefreshTokenDto
            {
                Id = t.Id,
                UserId = t.UserId,
                ExpiresAt = t.ExpiresAt,
                IsRevoked = t.IsRevoked,
                IsUsed = t.IsUsed,
                TokenPreview = MaskToken(t.Token)
            })
            .ToList();

        return new PagedResult<RefreshTokenDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
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
