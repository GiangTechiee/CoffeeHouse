using CoffeeHouse.Application.PurchaseOrders.DTOs;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.PurchaseOrders.Commands.UpdatePurchaseOrder;

public class UpdatePurchaseOrderCommandHandler : IRequestHandler<UpdatePurchaseOrderCommand, PurchaseOrderDetailDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePurchaseOrderCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PurchaseOrderDetailDto> Handle(UpdatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.PurchaseOrders.GetWithDetailsAsync(request.PurchaseOrderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException($"PurchaseOrder with ID {request.PurchaseOrderId} not found");
        }

        var store = await _unitOfWork.CafeStores.GetByIdAsync(request.StoreId, cancellationToken);
        if (store == null) throw new NotFoundException($"CafeStore with ID {request.StoreId} not found");

        var employee = await _unitOfWork.Employees.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee == null) throw new NotFoundException($"Employee with ID {request.EmployeeId} not found");

        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier == null) throw new NotFoundException($"Supplier with ID {request.SupplierId} not found");

        order.StoreId = request.StoreId;
        order.EmployeeId = request.EmployeeId;
        order.SupplierId = request.SupplierId;
        order.OrderDate = request.OrderDate;
        order.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        order.Status = string.IsNullOrWhiteSpace(request.Status) ? order.Status : request.Status.Trim();

        _unitOfWork.PurchaseOrders.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new PurchaseOrderDetailDto
        {
            PurchaseOrderId = order.PurchaseOrderId,
            StoreId = order.StoreId,
            StoreName = store.StoreName,
            EmployeeId = order.EmployeeId,
            EmployeeName = employee.FullName,
            SupplierId = order.SupplierId,
            SupplierName = supplier.SupplierName,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            Description = order.Description,
            Items = order.PurchaseOrderItems.Select(i => new PurchaseOrderItemDto
            {
                IngredientId = i.IngredientId,
                IngredientName = i.Ingredient.IngredientName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                LineTotal = i.LineTotal
            }).ToList()
        };
    }
}
