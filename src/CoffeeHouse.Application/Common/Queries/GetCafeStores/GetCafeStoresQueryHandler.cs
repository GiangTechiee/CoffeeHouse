using CoffeeHouse.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoffeeHouse.Application.Common.Queries.GetCafeStores;

public class GetCafeStoresQueryHandler : IRequestHandler<GetCafeStoresQuery, List<CafeStoreDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCafeStoresQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<CafeStoreDto>> Handle(GetCafeStoresQuery request, CancellationToken cancellationToken)
    {
        var stores = await _unitOfWork.CafeStores.GetAllAsync(cancellationToken);
        return stores.Select(s => new CafeStoreDto
        {
            StoreId = s.StoreId,
            StoreName = s.StoreName
        }).ToList();
    }
}
