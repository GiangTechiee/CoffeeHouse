using MediatR;
using System.Collections.Generic;

namespace CoffeeHouse.Application.Common.Queries.GetCafeStores;

public class GetCafeStoresQuery : IRequest<List<CafeStoreDto>>
{
}

public class CafeStoreDto
{
    public int StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;
}
