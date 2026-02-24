using CoffeeHouse.Application.CafeStores.DTOs;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.CafeStores.Commands.UpdateCafeStore;

public class UpdateCafeStoreCommandHandler : IRequestHandler<UpdateCafeStoreCommand, CafeStoreDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCafeStoreCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CafeStoreDto> Handle(UpdateCafeStoreCommand request, CancellationToken cancellationToken)
    {
        var store = await _unitOfWork.CafeStores.GetByIdAsync(request.StoreId, cancellationToken);
        if (store == null)
        {
            throw new NotFoundException($"CafeStore with ID {request.StoreId} not found");
        }

        store.StoreName = request.StoreName.Trim();
        store.Address = request.Address.Trim();
        store.PhoneNumber = request.PhoneNumber.Trim();
        store.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();

        _unitOfWork.CafeStores.Update(store);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
