using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Orders.DTOs;
using CoffeeHouse.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoffeeHouse.Application.Orders.Queries.GetOrders;

/// <summary>
/// Handler for GetOrdersQuery
/// </summary>
public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PagedResult<OrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetOrdersQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of GetOrdersQueryHandler
    /// </summary>
    /// <param name="unitOfWork">Unit of work for data access</param>
    /// <param name="logger">Logger instance</param>
    public GetOrdersQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetOrdersQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetOrdersQuery
    /// </summary>
    /// <param name="request">The query request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>PagedResult containing OrderDto list</returns>
    public async Task<PagedResult<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting orders - CustomerId: {CustomerId}, EmployeeId: {EmployeeId}, CoffeeShopId: {CoffeeShopId}, Status: {Status}, StartDate: {StartDate}, EndDate: {EndDate}, Page: {PageNumber}, PageSize: {PageSize}",
            request.CustomerId,
            request.EmployeeId,
            request.CoffeeShopId,
            request.Status,
            request.StartDate,
            request.EndDate,
            request.PageNumber,
            request.PageSize);

        // Validate pagination parameters
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;
        
        // Limit maximum page size to prevent performance issues
        if (pageSize > 100)
        {
            _logger.LogWarning("Page size {RequestedPageSize} exceeds maximum, limiting to 100", pageSize);
            pageSize = 100;
        }

        // Get paged orders from database
        var (pagedOrders, totalCount) = await _unitOfWork.Orders.GetPagedAsync(
            request.CustomerId,
            request.EmployeeId,
            request.CoffeeShopId,
            request.Status,
            request.StartDate,
            request.EndDate,
            pageNumber,
            pageSize,
            cancellationToken);

        _logger.LogInformation(
            "Retrieved {PagedOrderCount} orders out of {TotalCount} total for page {PageNumber}",
            pagedOrders.Count,
            totalCount,
            pageNumber);

        // Map orders to DTOs
        var orderDtos = pagedOrders.Select(MapToDto).ToList();

        // Create paged result
        var pagedResult = new PagedResult<OrderDto>
        {
            Items = orderDtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return pagedResult;
    }

    /// <summary>
    /// Maps Order entity to OrderDto
    /// </summary>
    /// <param name="order">Order entity</param>
    /// <returns>OrderDto</returns>
    private static OrderDto MapToDto(Domain.Entities.SalesOrder order)
    {
        return new OrderDto
        {
            // Id = order.Id, // SalesOrder does not have integer Id
            OrderId = order.OrderId,
            CoffeeShopId = order.StoreId,
            OrderDate = order.OrderDate,
            EmployeeId = order.EmployeeId,
            CustomerId = order.CustomerId,
            PaymentMethod = order.PaymentMethod,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            Items = order.SalesOrderItems.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.Product?.ProductName ?? "Unknown",
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.LineTotal
            }).ToList(),
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };
    }
}
