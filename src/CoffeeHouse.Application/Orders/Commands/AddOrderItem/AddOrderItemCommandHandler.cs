using CoffeeHouse.Application.Common.Mappings;
using CoffeeHouse.Application.Orders.DTOs;
using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace CoffeeHouse.Application.Orders.Commands.AddOrderItem;

/// <summary>
/// Handler for AddOrderItemCommand
/// </summary>
public class AddOrderItemCommandHandler : IRequestHandler<AddOrderItemCommand, OrderDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AddOrderItemCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of AddOrderItemCommandHandler
    /// </summary>
    /// <param name="unitOfWork">Unit of work for data access</param>
    /// <param name="logger">Logger instance</param>
    public AddOrderItemCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<AddOrderItemCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the AddOrderItemCommand
    /// </summary>
    /// <param name="request">The command request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>OrderDto representing the updated order</returns>
    /// <exception cref="NotFoundException">Thrown when order or product is not found</exception>
    /// <exception cref="DomainException">Thrown when business rules are violated</exception>
    public async Task<OrderDto> Handle(AddOrderItemCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Adding item to order: OrderId={OrderId}, ProductId={ProductId}, Quantity={Quantity}",
            request.OrderId,
            request.ProductId,
            request.Quantity);

        // Validate product exists
        var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
        {
            _logger.LogWarning("Product not found: {ProductId}", request.ProductId);
            throw new NotFoundException($"Product with ID {request.ProductId} not found");
        }

        _logger.LogDebug(
            "Product found: {ProductName}, Price={Price}",
            product.ProductName,
            product.Price);

        // Get the order with items
        var order = await _unitOfWork.Orders.GetWithItemsAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            _logger.LogWarning("Order not found: {OrderId}", request.OrderId);
            throw new NotFoundException($"Order with ID {request.OrderId} not found");
        }

        _logger.LogDebug(
            "Order found: OrderId={OrderId}, Status={Status}, CurrentTotal={CurrentTotal}",
            order.OrderId,
            order.Status,
            order.TotalAmount);

        // Add item to order
        try
        {
            order.AddItem(
                productId: product.ProductId,
                quantity: request.Quantity,
                unitPrice: product.Price
            );

            _logger.LogInformation(
                "Item added successfully: Product={ProductName}, Quantity={Quantity}, UnitPrice={UnitPrice}",
                product.ProductName,
                request.Quantity,
                product.Price);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to add item to order: OrderId={OrderId}, ProductId={ProductId}",
                request.OrderId,
                request.ProductId);
            throw;
        }

        // Update order in repository (mark as modified)
        _unitOfWork.Orders.Update(order);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Order updated successfully: OrderId={OrderId}, NewTotal={NewTotal}",
            order.OrderId,
            order.TotalAmount);

        // Map to DTO and return
        return _mapper.Map<OrderDto>(order);
    }
}
