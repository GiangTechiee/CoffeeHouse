using CoffeeHouse.Application.Orders.DTOs;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.Orders.Commands.UpdateAdminOrderStatus;

public class UpdateAdminOrderStatusCommandHandler : IRequestHandler<UpdateAdminOrderStatusCommand, AdminSalesOrderDetailDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAdminOrderStatusCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AdminSalesOrderDetailDto> Handle(UpdateAdminOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetWithItemsAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException($"SalesOrder with ID {request.OrderId} not found");
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            order.Status = NormalizeStatus(request.Status);
        }

        if (request.EmployeeId.HasValue)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(request.EmployeeId.Value, cancellationToken);
            if (employee == null)
            {
                throw new NotFoundException($"Employee with ID {request.EmployeeId.Value} not found");
            }
            order.EmployeeId = request.EmployeeId.Value;
        }

        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AdminSalesOrderDetailDto
        {
            OrderId = order.OrderId,
            StoreId = order.StoreId,
            StoreName = order.Store?.StoreName ?? string.Empty,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer?.CustomerName ?? string.Empty,
            EmployeeId = order.EmployeeId,
            EmployeeName = order.Employee?.FullName,
            OrderDate = order.OrderDate,
            PaymentMethod = order.PaymentMethod,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            Items = order.SalesOrderItems.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.Product?.ProductName ?? string.Empty,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.LineTotal
            }).ToList()
        };
    }

    private static string NormalizeStatus(string status)
    {
        var trimmed = status.Trim();
        return trimmed switch
        {
            "Chua hoàn thành" => "Pending",
            "Hoàn thành" => "Completed",
            _ => trimmed
        };
    }
}
