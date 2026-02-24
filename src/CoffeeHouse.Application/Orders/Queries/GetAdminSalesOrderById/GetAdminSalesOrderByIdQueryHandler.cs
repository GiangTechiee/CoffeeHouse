using CoffeeHouse.Application.Orders.DTOs;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.Orders.Queries.GetAdminSalesOrderById;

public class GetAdminSalesOrderByIdQueryHandler : IRequestHandler<GetAdminSalesOrderByIdQuery, AdminSalesOrderDetailDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAdminSalesOrderByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AdminSalesOrderDetailDto> Handle(GetAdminSalesOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetWithItemsAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException($"SalesOrder with ID {request.OrderId} not found");
        }

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
}
