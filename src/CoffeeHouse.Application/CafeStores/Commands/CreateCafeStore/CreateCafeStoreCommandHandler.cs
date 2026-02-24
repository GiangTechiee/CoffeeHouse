using CoffeeHouse.Application.CafeStores.DTOs;
using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.CafeStores.Commands.CreateCafeStore;

public class CreateCafeStoreCommandHandler : IRequestHandler<CreateCafeStoreCommand, CafeStoreDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCafeStoreCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CafeStoreDto> Handle(CreateCafeStoreCommand request, CancellationToken cancellationToken)
    {
        var store = new CafeStore
        {
            StoreName = request.StoreName.Trim(),
            Address = request.Address.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim()
        };

        await _unitOfWork.CafeStores.AddAsync(store, cancellationToken);
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
