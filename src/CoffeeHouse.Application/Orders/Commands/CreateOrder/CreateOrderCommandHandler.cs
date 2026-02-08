using AutoMapper;
using CoffeeHouse.Application.Orders.DTOs;
using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoffeeHouse.Application.Orders.Commands.CreateOrder;

/// <summary>
/// Handler for CreateOrderCommand
/// </summary>
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of CreateOrderCommandHandler
    /// </summary>
    /// <param name="unitOfWork">Unit of work for data access</param>
    /// <param name="logger">Logger instance</param>
    public CreateOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the CreateOrderCommand
    /// </summary>
    /// <param name="request">The command request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>OrderDto representing the created order</returns>
    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating order for customer: {CustomerId}, coffeeShop: {CoffeeShopId}, payment: {PaymentMethod}",
            request.CustomerId,
            request.CoffeeShopId,
            request.PaymentMethod);

        // Validate that order has items
        if (request.Items == null || !request.Items.Any())
        {
            _logger.LogWarning("Attempted to create order with no items");
            throw new InvalidOperationException("Order must contain at least one item");
        }

        // Validate all items
        ValidateOrderItems(request.Items);

        // Create domain entity
        var order = new SalesOrder
        {
            OrderId = Guid.NewGuid(),
            StoreId = request.CoffeeShopId,
            CustomerId = request.CustomerId,
            PaymentMethod = request.PaymentMethod,
            EmployeeId = request.EmployeeId,
            OrderDate = DateTime.UtcNow,
            Status = "Pending"
        };

        // Add items to the order - fetch real prices from DB to prevent tampering
        foreach (var item in request.Items)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId, cancellationToken);
            if (product == null)
            {
                 _logger.LogWarning("Product {ProductId} not found while creating order", item.ProductId);
                 throw new NotFoundException(nameof(Product), item.ProductId);
            }

            order.AddItem(
                productId: product.ProductId,
                quantity: item.Quantity,
                unitPrice: product.Price
            );
        }

        // Add order to repository
        await _unitOfWork.Orders.AddAsync(order, cancellationToken);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Order created successfully with ID: {OrderId}, Total: {Total}",
            order.OrderId,
            order.TotalAmount);

        // Map to DTO and return
        return _mapper.Map<OrderDto>(order);
    }

    /// <summary>
    /// Validates order items
    /// </summary>
    /// <param name="items">List of order items to validate</param>
    private void ValidateOrderItems(List<CreateOrderItemDto> items)
    {
        // Check for duplicate product IDs
        var duplicateProductIds = items
            .GroupBy(i => i.ProductId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateProductIds.Any())
        {
            var duplicateIds = string.Join(", ", duplicateProductIds);
            _logger.LogWarning("Order contains duplicate product IDs: {DuplicateIds}", duplicateIds);
            throw new InvalidOperationException($"Order contains duplicate products: {duplicateIds}");
        }

        // Validate each item
        foreach (var item in items)
        {
            if (item.ProductId <= 0)
            {
                throw new InvalidOperationException($"Invalid product ID: {item.ProductId}");
            }

            if (item.Quantity <= 0)
            {
                throw new InvalidOperationException($"Quantity must be greater than zero for product ID: {item.ProductId}");
            }
        }
    }
}
