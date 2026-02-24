using CoffeeHouse.Application.PurchaseOrders.DTOs;
using MediatR;

namespace CoffeeHouse.Application.PurchaseOrders.Queries.GetPurchaseOrderById;

public record GetPurchaseOrderByIdQuery(Guid PurchaseOrderId) : IRequest<PurchaseOrderDetailDto>;
