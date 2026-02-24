using CoffeeHouse.Application.Orders.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Orders.Commands.UpdateAdminOrderStatus;

public record UpdateAdminOrderStatusCommand(Guid OrderId, string? Status, int? EmployeeId) : IRequest<AdminSalesOrderDetailDto>;
