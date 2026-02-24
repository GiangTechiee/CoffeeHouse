using CoffeeHouse.Application.PurchaseOrders.DTOs;
using MediatR;

namespace CoffeeHouse.Application.PurchaseOrders.Commands.CreatePurchaseOrder;

public record CreatePurchaseOrderCommand(
    int StoreId,
    int EmployeeId,
    int SupplierId,
    DateTime? OrderDate,
    string? Description,
    string? Status,
    List<PurchaseOrderItemInput> Items) : IRequest<PurchaseOrderDetailDto>;
