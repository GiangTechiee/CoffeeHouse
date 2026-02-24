using CoffeeHouse.Application.PurchaseOrders.DTOs;
using MediatR;

namespace CoffeeHouse.Application.PurchaseOrders.Commands.UpdatePurchaseOrderItem;

public record UpdatePurchaseOrderItemCommand(
    Guid PurchaseOrderId,
    int IngredientId,
    decimal Quantity,
    decimal UnitPrice) : IRequest<PurchaseOrderDetailDto>;
