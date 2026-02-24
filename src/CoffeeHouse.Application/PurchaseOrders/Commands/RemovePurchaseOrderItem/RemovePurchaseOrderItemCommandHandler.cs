using CoffeeHouse.Application.PurchaseOrders.DTOs;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.PurchaseOrders.Commands.RemovePurchaseOrderItem;

public class RemovePurchaseOrderItemCommandHandler : IRequestHandler<RemovePurchaseOrderItemCommand, PurchaseOrderDetailDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemovePurchaseOrderItemCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PurchaseOrderDetailDto> Handle(RemovePurchaseOrderItemCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.PurchaseOrders.GetForUpdateAsync(request.PurchaseOrderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException($"PurchaseOrder with ID {request.PurchaseOrderId} not found");
        }

        var item = order.PurchaseOrderItems.FirstOrDefault(i => i.IngredientId == request.IngredientId);
        if (item == null)
        {
            throw new NotFoundException($"PurchaseOrderItem with Ingredient ID {request.IngredientId} not found");
        }

        order.PurchaseOrderItems.Remove(item);

        order.TotalAmount = order.PurchaseOrderItems.Sum(i => i.Quantity * i.UnitPrice);

        _unitOfWork.PurchaseOrders.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new PurchaseOrderDetailDto
        {
            PurchaseOrderId = order.PurchaseOrderId,
            StoreId = order.StoreId,
            StoreName = order.Store?.StoreName ?? string.Empty,
            EmployeeId = order.EmployeeId,
            EmployeeName = order.Employee?.FullName ?? string.Empty,
            SupplierId = order.SupplierId,
            SupplierName = order.Supplier?.SupplierName ?? string.Empty,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            Description = order.Description,
            Items = order.PurchaseOrderItems.Select(i => new PurchaseOrderItemDto
            {
                IngredientId = i.IngredientId,
                IngredientName = i.Ingredient?.IngredientName ?? string.Empty,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                LineTotal = i.Quantity * i.UnitPrice
            }).ToList()
        };
    }
}
