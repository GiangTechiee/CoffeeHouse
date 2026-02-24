using CoffeeHouse.Application.CafeStores.DTOs;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.CafeStores.Queries.GetCafeStoreById;

public class GetCafeStoreByIdQueryHandler : IRequestHandler<GetCafeStoreByIdQuery, CafeStoreDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCafeStoreByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CafeStoreDto> Handle(GetCafeStoreByIdQuery request, CancellationToken cancellationToken)
    {
        var store = await _unitOfWork.CafeStores.GetByIdAsync(request.StoreId, cancellationToken);
        if (store == null)
        {
            throw new NotFoundException($"CafeStore with ID {request.StoreId} not found");
        }

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
