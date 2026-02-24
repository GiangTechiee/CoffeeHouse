using CoffeeHouse.Application.PurchaseOrders.DTOs;
using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.PurchaseOrders.Commands.CreatePurchaseOrder;

public class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, PurchaseOrderDetailDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreatePurchaseOrderCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PurchaseOrderDetailDto> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.Items == null || request.Items.Count == 0)
        {
            throw new InvalidOperationException("Purchase order must contain at least one item.");
        }

        var store = await _unitOfWork.CafeStores.GetByIdAsync(request.StoreId, cancellationToken);
        if (store == null) throw new NotFoundException($"CafeStore with ID {request.StoreId} not found");

        var employee = await _unitOfWork.Employees.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee == null) throw new NotFoundException($"Employee with ID {request.EmployeeId} not found");

        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier == null) throw new NotFoundException($"Supplier with ID {request.SupplierId} not found");

        var groupedItems = request.Items
            .Where(i => i.Quantity > 0)
            .GroupBy(i => i.IngredientId)
            .Select(g => new PurchaseOrderItemInput
            {
                IngredientId = g.Key,
                Quantity = g.Sum(x => x.Quantity),
                UnitPrice = g.Last().UnitPrice
            })
            .ToList();

        if (groupedItems.Count == 0)
        {
            throw new InvalidOperationException("Purchase order must contain at least one item.");
        }

        var order = new PurchaseOrder
        {
            PurchaseOrderId = Guid.NewGuid(),
            StoreId = request.StoreId,
            EmployeeId = request.EmployeeId,
            SupplierId = request.SupplierId,
            OrderDate = request.OrderDate ?? DateTime.UtcNow,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Status = string.IsNullOrWhiteSpace(request.Status) ? "Pending" : request.Status.Trim()
        };

        decimal totalAmount = 0;
        var detailItems = new List<PurchaseOrderItemDto>();

        foreach (var item in groupedItems)
        {
            var ingredient = await _unitOfWork.Ingredients.GetByIdAsync(item.IngredientId, cancellationToken);
            if (ingredient == null)
            {
                throw new NotFoundException($"Ingredient with ID {item.IngredientId} not found");
            }

            var lineTotal = item.Quantity * item.UnitPrice;
            totalAmount += lineTotal;

            order.PurchaseOrderItems.Add(new PurchaseOrderItem
            {
                PurchaseOrderId = order.PurchaseOrderId,
                IngredientId = item.IngredientId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            });

            ingredient.Quantity += item.Quantity;
            _unitOfWork.Ingredients.Update(ingredient);

            detailItems.Add(new PurchaseOrderItemDto
            {
                IngredientId = ingredient.IngredientId,
                IngredientName = ingredient.IngredientName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = lineTotal
            });
        }

        order.TotalAmount = totalAmount;

        await _unitOfWork.PurchaseOrders.AddAsync(order, cancellationToken);
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
            Items = detailItems
        };
    }
}
