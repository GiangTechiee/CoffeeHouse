using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoffeeHouse.Application.Carts.Commands.RemoveFromCart;

/// <summary>
/// Handler for RemoveFromCartCommand
/// </summary>
public class RemoveFromCartCommandHandler : IRequestHandler<RemoveFromCartCommand, RemoveFromCartResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemoveFromCartCommandHandler> _logger;

    public RemoveFromCartCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<RemoveFromCartCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<RemoveFromCartResult> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Removing product {ProductId} from cart for customer {CustomerId}",
            request.ProductId,
            request.CustomerId);

        var cartItem = await _unitOfWork.Carts.GetCartItemAsync(
            request.CustomerId,
            request.ProductId,
            cancellationToken);

        if (cartItem == null)
        {
            throw new NotFoundException($"Cart item not found for product ID {request.ProductId}");
        }

        _unitOfWork.Carts.Remove(cartItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var cartItems = await _unitOfWork.Carts.GetCartItemsAsync(request.CustomerId, cancellationToken);
        var totalItems = cartItems.Sum(item => item.Quantity);
        var totalAmount = cartItems.Sum(item => item.Quantity * (item.Product?.Price ?? 0));

        return new RemoveFromCartResult
        {
            Success = true,
            Message = "Product removed from cart successfully",
            TotalItems = totalItems,
            TotalAmount = totalAmount
        };
    }
}
