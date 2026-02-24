using CoffeeHouse.Application.CafeStores.DTOs;
using MediatR;

namespace CoffeeHouse.Application.CafeStores.Queries.GetCafeStoreById;

public record GetCafeStoreByIdQuery(int StoreId) : IRequest<CafeStoreDto>;
