using CoffeeHouse.Application.Orders.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Orders.Queries.GetAdminSalesOrderById;

public record GetAdminSalesOrderByIdQuery(Guid OrderId) : IRequest<AdminSalesOrderDetailDto>;
