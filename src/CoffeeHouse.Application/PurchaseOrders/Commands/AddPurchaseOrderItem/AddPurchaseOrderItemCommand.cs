using CoffeeHouse.Application.PurchaseOrders.DTOs;
using MediatR;

namespace CoffeeHouse.Application.PurchaseOrders.Commands.AddPurchaseOrderItem;

public record AddPurchaseOrderItemCommand(
    Guid PurchaseOrderId,
    int IngredientId,
    decimal Quantity,
    decimal UnitPrice) : IRequest<PurchaseOrderDetailDto>;
