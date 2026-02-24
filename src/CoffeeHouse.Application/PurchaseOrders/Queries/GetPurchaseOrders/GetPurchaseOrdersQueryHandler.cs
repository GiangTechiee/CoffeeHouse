using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.PurchaseOrders.DTOs;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.PurchaseOrders.Queries.GetPurchaseOrders;

public class GetPurchaseOrdersQueryHandler : IRequestHandler<GetPurchaseOrdersQuery, PagedResult<PurchaseOrderListItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPurchaseOrdersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<PurchaseOrderListItemDto>> Handle(GetPurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : request.PageSize;
        if (pageSize > 100) pageSize = 100;

        var (items, totalCount) = await _unitOfWork.PurchaseOrders.GetPagedAsync(
            request.SearchDate,
            pageNumber,
            pageSize,
            cancellationToken);

        var dtos = items.Select(po => new PurchaseOrderListItemDto
        {
            PurchaseOrderId = po.PurchaseOrderId,
            StoreId = po.StoreId,
            StoreName = po.Store.StoreName,
            EmployeeId = po.EmployeeId,
            EmployeeName = po.Employee.FullName,
            SupplierId = po.SupplierId,
            SupplierName = po.Supplier.SupplierName,
            OrderDate = po.OrderDate,
            TotalAmount = po.TotalAmount,
            Status = po.Status,
            Description = po.Description
        }).ToList();

        return new PagedResult<PurchaseOrderListItemDto>
        {
            Items = dtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
