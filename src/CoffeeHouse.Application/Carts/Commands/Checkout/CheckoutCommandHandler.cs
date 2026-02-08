using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoffeeHouse.Application.Carts.Commands.Checkout;

/// <summary>
/// Handler for CheckoutCommand
/// </summary>
public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, CheckoutResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CheckoutCommandHandler> _logger;

    public CheckoutCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CheckoutCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CheckoutResult> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Starting checkout for customer {CustomerId} at coffee shop {CoffeeShopId}",
            request.CustomerId,
            request.CoffeeShopId);

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Get cart items
            var cartItems = await _unitOfWork.Carts.GetCartItemsAsync(request.CustomerId, cancellationToken);
            
            if (cartItems == null || !cartItems.Any())
            {
                throw new InvalidOperationException("Cart is empty. Cannot proceed with checkout.");
            }

            // Verify customer exists
            var customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId, cancellationToken);
            if (customer == null)
            {
                throw new NotFoundException($"Customer with ID {request.CustomerId} not found");
            }

            // Calculate total
            var totalAmount = cartItems.Sum(item => item.Quantity * (item.Product?.Price ?? 0));

            // Create order
            var order = new SalesOrder
            {
                OrderId = Guid.NewGuid(),
                StoreId = request.CoffeeShopId,
                OrderDate = DateTime.UtcNow,
                EmployeeId = request.EmployeeId,
                CustomerId = request.CustomerId,
                PaymentMethod = request.PaymentMethod ?? "Unknown",
                TotalAmount = totalAmount,
                Status = "Chưa hoàn thành"
            };

            // Add order details
            foreach (var cartItem in cartItems)
            {
                var orderDetail = new SalesOrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = cartItem.ProductId,
                    UnitPrice = cartItem.Product?.Price ?? 0,
                    Quantity = cartItem.Quantity
                    // LineTotal is computed by the database
                };
                
                order.SalesOrderItems.Add(orderDetail);
            }

            // Add order through UnitOfWork
            await _unitOfWork.Orders.AddAsync(order, cancellationToken);

            // Clear cart
            await _unitOfWork.Carts.RemoveAllAsync(request.CustomerId, cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully completed checkout for customer {CustomerId}. Order ID: {OrderId}",
                request.CustomerId,
                order.OrderId);

            return new CheckoutResult
            {
                Success = true,
                Message = "Checkout completed successfully",
                OrderId = order.OrderId,
                TotalAmount = totalAmount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error during checkout for customer {CustomerId}",
                request.CustomerId);

            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
