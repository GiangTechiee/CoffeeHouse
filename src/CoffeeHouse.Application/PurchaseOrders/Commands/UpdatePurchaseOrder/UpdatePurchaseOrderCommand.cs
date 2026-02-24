using CoffeeHouse.Application.PurchaseOrders.DTOs;
using MediatR;

namespace CoffeeHouse.Application.PurchaseOrders.Commands.UpdatePurchaseOrder;

public record UpdatePurchaseOrderCommand(
    Guid PurchaseOrderId,
    int StoreId,
    int EmployeeId,
    int SupplierId,
    DateTime OrderDate,
    string? Description,
    string? Status) : IRequest<PurchaseOrderDetailDto>;
