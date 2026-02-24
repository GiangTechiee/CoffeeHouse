using CoffeeHouse.Application.PurchaseOrders.DTOs;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.PurchaseOrders.Queries.GetPurchaseOrderById;

public class GetPurchaseOrderByIdQueryHandler : IRequestHandler<GetPurchaseOrderByIdQuery, PurchaseOrderDetailDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPurchaseOrderByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PurchaseOrderDetailDto> Handle(GetPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.PurchaseOrders.GetWithDetailsAsync(request.PurchaseOrderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException($"PurchaseOrder with ID {request.PurchaseOrderId} not found");
        }

        return new PurchaseOrderDetailDto
        {
            PurchaseOrderId = order.PurchaseOrderId,
            StoreId = order.StoreId,
            StoreName = order.Store.StoreName,
            EmployeeId = order.EmployeeId,
            EmployeeName = order.Employee.FullName,
            SupplierId = order.SupplierId,
            SupplierName = order.Supplier.SupplierName,
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
