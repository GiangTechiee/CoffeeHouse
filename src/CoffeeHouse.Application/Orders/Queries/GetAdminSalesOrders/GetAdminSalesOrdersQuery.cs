using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Orders.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Orders.Queries.GetAdminSalesOrders;

public record GetAdminSalesOrdersQuery(int PageNumber = 1, int PageSize = 20, DateTime? SearchDate = null)
    : IRequest<PagedResult<AdminSalesOrderListItemDto>>;
