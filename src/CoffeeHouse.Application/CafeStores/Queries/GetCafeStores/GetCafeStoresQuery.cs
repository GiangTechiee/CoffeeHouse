using CoffeeHouse.Application.CafeStores.DTOs;
using CoffeeHouse.Application.Common.Models;
using MediatR;

namespace CoffeeHouse.Application.CafeStores.Queries.GetCafeStores;

public record GetCafeStoresQuery(int PageNumber = 1, int PageSize = 20, string? SearchTerm = null)
    : IRequest<PagedResult<CafeStoreDto>>;
