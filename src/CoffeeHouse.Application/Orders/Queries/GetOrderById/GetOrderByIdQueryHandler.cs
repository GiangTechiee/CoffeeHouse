using CoffeeHouse.Application.Orders.DTOs;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoffeeHouse.Application.Orders.Queries.GetOrderById;

/// <summary>
/// Handler for GetOrderByIdQuery
/// </summary>
public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetOrderByIdQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of GetOrderByIdQueryHandler
    /// </summary>
    /// <param name="unitOfWork">Unit of work for data access</param>
    /// <param name="logger">Logger instance</param>
    public GetOrderByIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetOrderByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetOrderByIdQuery
    /// </summary>
    /// <param name="request">The query request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>OrderDto representing the requested order</returns>
    /// <exception cref="NotFoundException">Thrown when order is not found</exception>
    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting order by ID: {OrderId}, IncludeItems: {IncludeItems}",
            request.OrderId,
            request.IncludeItems);

        // Get order with or without items based on request
        Domain.Entities.SalesOrder? order;
        
        if (request.IncludeItems)
        {
            order = await _unitOfWork.Orders.GetWithItemsAsync(request.OrderId, cancellationToken);
        }
        else
        {
            order = await _unitOfWork.Orders.GetByOrderIdAsync(request.OrderId, cancellationToken);
        }

        if (order == null)
        {
            _logger.LogWarning("Order not found: {OrderId}", request.OrderId);
            throw new NotFoundException($"Order with ID {request.OrderId} not found");
        }

        _logger.LogInformation(
            "Order found: OrderId={OrderId}, Status={Status}, Total={Total}, ItemCount={ItemCount}",
            order.OrderId,
            order.Status,
            order.TotalAmount,
            order.SalesOrderItems.Count);

        // Map to DTO and return
        return MapToDto(order);
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
