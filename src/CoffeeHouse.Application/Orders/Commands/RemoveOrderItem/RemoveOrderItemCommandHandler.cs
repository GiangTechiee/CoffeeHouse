using CoffeeHouse.Application.Orders.DTOs;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using CoffeeHouse.Domain.Entities;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace CoffeeHouse.Application.Orders.Commands.RemoveOrderItem;

/// <summary>
/// Handler for RemoveOrderItemCommand
/// </summary>
public class RemoveOrderItemCommandHandler : IRequestHandler<RemoveOrderItemCommand, OrderDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<RemoveOrderItemCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of RemoveOrderItemCommandHandler
    /// </summary>
    /// <param name="unitOfWork">Unit of work for data access</param>
    /// <param name="logger">Logger instance</param>
    public RemoveOrderItemCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<RemoveOrderItemCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the RemoveOrderItemCommand
    /// </summary>
    /// <param name="request">The command request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>OrderDto representing the updated order</returns>
    /// <exception cref="NotFoundException">Thrown when order is not found</exception>
    /// <exception cref="DomainException">Thrown when business rules are violated</exception>
    public async Task<OrderDto> Handle(RemoveOrderItemCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Removing item from order: OrderId={OrderId}, ProductId={ProductId}",
            request.OrderId,
            request.ProductId);

        // Get the order with items
        var order = await _unitOfWork.Orders.GetWithItemsAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            _logger.LogWarning("Order not found: {OrderId}", request.OrderId);
            throw new NotFoundException($"Order with ID {request.OrderId} not found");
        }

        _logger.LogDebug(
            "Order found: OrderId={OrderId}, Status={Status}, CurrentTotal={CurrentTotal}, ItemCount={ItemCount}",
            order.OrderId,
            order.Status,
            order.TotalAmount,
            order.SalesOrderItems.Count);

        // Remove item from order
        try
        {
            order.RemoveItem(request.ProductId);

            _logger.LogInformation(
                "Item removed successfully: ProductId={ProductId}",
                request.ProductId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to remove item from order: OrderId={OrderId}, ProductId={ProductId}",
                request.OrderId,
                request.ProductId);
            throw;
        }

        // Update order in repository (mark as modified)
        _unitOfWork.Orders.Update(order);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Order updated successfully: OrderId={OrderId}, NewTotal={NewTotal}, RemainingItems={RemainingItems}",
            order.OrderId,
            order.TotalAmount,
            order.SalesOrderItems.Count);

        // Map to DTO and return
        return _mapper.Map<OrderDto>(order);
    }
}
