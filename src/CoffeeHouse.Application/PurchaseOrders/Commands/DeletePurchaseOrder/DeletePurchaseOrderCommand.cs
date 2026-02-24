using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.PurchaseOrders.Commands.DeletePurchaseOrder;

public record DeletePurchaseOrderCommand(Guid PurchaseOrderId) : IRequest;

public class DeletePurchaseOrderCommandHandler : IRequestHandler<DeletePurchaseOrderCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeletePurchaseOrderCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeletePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.PurchaseOrders.GetWithDetailsAsync(request.PurchaseOrderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException($"PurchaseOrder with ID {request.PurchaseOrderId} not found");
        }

        await _unitOfWork.PurchaseOrders.DeleteWithItemsAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
