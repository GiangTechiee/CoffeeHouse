using CoffeeHouse.Application.CafeStores.DTOs;
using MediatR;

namespace CoffeeHouse.Application.CafeStores.Commands.UpdateCafeStore;

public record UpdateCafeStoreCommand(
    int StoreId,
    string StoreName,
    string Address,
    string PhoneNumber,
    string? Email) : IRequest<CafeStoreDto>;
