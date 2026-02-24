using CoffeeHouse.Application.CafeStores.DTOs;
using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoffeeHouse.Application.CafeStores.Queries.GetCafeStores;

public class GetCafeStoresQueryHandler : IRequestHandler<GetCafeStoresQuery, PagedResult<CafeStoreDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetCafeStoresQueryHandler> _logger;

    public GetCafeStoresQueryHandler(IUnitOfWork unitOfWork, ILogger<GetCafeStoresQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PagedResult<CafeStoreDto>> Handle(GetCafeStoresQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : request.PageSize;
        if (pageSize > 100) pageSize = 100;

        var (items, totalCount) = await _unitOfWork.CafeStores.GetPagedAsync(
            request.SearchTerm,
            pageNumber,
            pageSize,
            cancellationToken);

        var dtos = items.Select(MapToDto).ToList();

        return new PagedResult<CafeStoreDto>
        {
            Items = dtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    private static CafeStoreDto MapToDto(Domain.Entities.CafeStore store)
    {
        return new CafeStoreDto
        {
            StoreId = store.StoreId,
            StoreName = store.StoreName,
            Address = store.Address,
            PhoneNumber = store.PhoneNumber,
            Email = store.Email
        };
    }
}
