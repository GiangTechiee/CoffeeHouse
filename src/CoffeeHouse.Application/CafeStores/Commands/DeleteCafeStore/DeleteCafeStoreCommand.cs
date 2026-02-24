using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.CafeStores.Commands.DeleteCafeStore;

public record DeleteCafeStoreCommand(int StoreId) : IRequest;

public class DeleteCafeStoreCommandHandler : IRequestHandler<DeleteCafeStoreCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCafeStoreCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteCafeStoreCommand request, CancellationToken cancellationToken)
    {
        var store = await _unitOfWork.CafeStores.GetByIdAsync(request.StoreId, cancellationToken);
        if (store == null)
        {
            throw new NotFoundException($"CafeStore with ID {request.StoreId} not found");
        }

        _unitOfWork.CafeStores.Delete(store);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
