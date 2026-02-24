using CoffeeHouse.Application.CafeStores.DTOs;
using MediatR;

namespace CoffeeHouse.Application.CafeStores.Commands.CreateCafeStore;

public record CreateCafeStoreCommand(
    string StoreName,
    string Address,
    string PhoneNumber,
    string? Email) : IRequest<CafeStoreDto>;
