using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.PurchaseOrders.DTOs;
using MediatR;

namespace CoffeeHouse.Application.PurchaseOrders.Queries.GetPurchaseOrders;

public record GetPurchaseOrdersQuery(int PageNumber = 1, int PageSize = 20, DateTime? SearchDate = null)
    : IRequest<PagedResult<PurchaseOrderListItemDto>>;
