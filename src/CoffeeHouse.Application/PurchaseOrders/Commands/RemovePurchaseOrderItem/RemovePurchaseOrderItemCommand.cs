using CoffeeHouse.Application.PurchaseOrders.DTOs;
using MediatR;

namespace CoffeeHouse.Application.PurchaseOrders.Commands.RemovePurchaseOrderItem;

public record RemovePurchaseOrderItemCommand(
    Guid PurchaseOrderId,
    int IngredientId) : IRequest<PurchaseOrderDetailDto>;
