using CoffeeHouse.Application.PurchaseOrders.DTOs;
using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.PurchaseOrders.Commands.AddPurchaseOrderItem;

public class AddPurchaseOrderItemCommandHandler : IRequestHandler<AddPurchaseOrderItemCommand, PurchaseOrderDetailDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddPurchaseOrderItemCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PurchaseOrderDetailDto> Handle(AddPurchaseOrderItemCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.PurchaseOrders.GetForUpdateAsync(request.PurchaseOrderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException($"PurchaseOrder with ID {request.PurchaseOrderId} not found");
        }

        var ingredient = await _unitOfWork.Ingredients.GetByIdAsync(request.IngredientId, cancellationToken);
        if (ingredient == null)
        {
            throw new NotFoundException($"Ingredient with ID {request.IngredientId} not found");
        }

        var existing = order.PurchaseOrderItems.FirstOrDefault(i => i.IngredientId == request.IngredientId);
        if (existing == null)
        {
            order.PurchaseOrderItems.Add(new PurchaseOrderItem
            {
                PurchaseOrderId = order.PurchaseOrderId,
                IngredientId = request.IngredientId,
                Quantity = request.Quantity,
                UnitPrice = request.UnitPrice
            });
        }
        else
        {
            existing.Quantity = request.Quantity;
            existing.UnitPrice = request.UnitPrice;
        }

        order.TotalAmount = order.PurchaseOrderItems.Sum(i => i.Quantity * i.UnitPrice);

        _unitOfWork.PurchaseOrders.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDetailDto(order);
    }

    private static PurchaseOrderDetailDto MapToDetailDto(PurchaseOrder order)
    {
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
