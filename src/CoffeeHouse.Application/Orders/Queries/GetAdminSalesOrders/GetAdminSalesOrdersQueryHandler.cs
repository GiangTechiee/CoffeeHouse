using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Orders.DTOs;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.Orders.Queries.GetAdminSalesOrders;

public class GetAdminSalesOrdersQueryHandler : IRequestHandler<GetAdminSalesOrdersQuery, PagedResult<AdminSalesOrderListItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAdminSalesOrdersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<AdminSalesOrderListItemDto>> Handle(GetAdminSalesOrdersQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : request.PageSize;
        if (pageSize > 100) pageSize = 100;

        DateTime? startDate = null;
        DateTime? endDate = null;
        if (request.SearchDate.HasValue)
        {
            var date = request.SearchDate.Value.Date;
            startDate = date;
            endDate = date.AddDays(1).AddTicks(-1);
        }

        var (orders, totalCount) = await _unitOfWork.Orders.GetPagedAsync(
            customerId: null,
            employeeId: null,
            storeId: null,
            status: null,
            startDate: startDate,
            endDate: endDate,
            pageNumber: pageNumber,
            pageSize: pageSize,
            cancellationToken: cancellationToken);

        var dtos = orders.Select(o => new AdminSalesOrderListItemDto
        {
            OrderId = o.OrderId,
            StoreId = o.StoreId,
            StoreName = o.Store?.StoreName ?? string.Empty,
            CustomerId = o.CustomerId,
            CustomerName = o.Customer?.CustomerName ?? string.Empty,
            EmployeeId = o.EmployeeId,
            EmployeeName = o.Employee?.FullName,
            OrderDate = o.OrderDate,
            PaymentMethod = o.PaymentMethod,
            TotalAmount = o.TotalAmount,
            Status = o.Status
        }).ToList();

        return new PagedResult<AdminSalesOrderListItemDto>
        {
            Items = dtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
