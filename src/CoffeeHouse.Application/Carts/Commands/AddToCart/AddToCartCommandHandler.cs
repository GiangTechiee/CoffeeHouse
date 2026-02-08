using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoffeeHouse.Application.Carts.Commands.AddToCart;

/// <summary>
/// Handler for AddToCartCommand
/// </summary>
public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, AddToCartResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddToCartCommandHandler> _logger;

    public AddToCartCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<AddToCartCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AddToCartResult> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Adding product {ProductId} to cart for customer {CustomerId} with quantity {Quantity}",
            request.ProductId,
            request.CustomerId,
            request.Quantity);

        if (request.Quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than 0", nameof(request.Quantity));
        }

        var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
        {
            throw new NotFoundException($"Product with ID {request.ProductId} not found");
        }

        var customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer == null)
        {
            throw new NotFoundException($"Customer with ID {request.CustomerId} not found");
        }

        var existingCartItem = await _unitOfWork.Carts.GetCartItemAsync(
            request.CustomerId,
            request.ProductId,
            cancellationToken);

        if (existingCartItem != null)
        {
            existingCartItem.Quantity += request.Quantity;
            _unitOfWork.Carts.Update(existingCartItem);
        }
        else
        {
            var newCartItem = new CartItem
            {
                CustomerId = request.CustomerId,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            };
            
            await _unitOfWork.Carts.AddAsync(newCartItem, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var cartItems = await _unitOfWork.Carts.GetCartItemsAsync(request.CustomerId, cancellationToken);
        var totalItems = cartItems.Sum(item => item.Quantity);
        var totalAmount = cartItems.Sum(item => item.Quantity * (item.Product?.Price ?? 0));

        return new AddToCartResult
        {
            Success = true,
            Message = "Product added to cart successfully",
            TotalItems = totalItems,
            TotalAmount = totalAmount
        };
    }
}
