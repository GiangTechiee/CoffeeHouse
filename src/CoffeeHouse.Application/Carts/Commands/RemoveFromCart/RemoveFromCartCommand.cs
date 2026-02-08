using MediatR;

namespace CoffeeHouse.Application.Carts.Commands.RemoveFromCart;

/// <summary>
/// Command to remove a product from the shopping cart
/// </summary>
public record RemoveFromCartCommand : IRequest<RemoveFromCartResult>
{
    /// <summary>
    /// Customer ID
    /// </summary>
    public int CustomerId { get; init; }

    /// <summary>
    /// Product ID to remove from cart
    /// </summary>
    public int ProductId { get; init; }
}

/// <summary>
/// Result of removing item from cart
/// </summary>
public record RemoveFromCartResult
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Message describing the result
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Total number of items in cart after operation
    /// </summary>
    public int TotalItems { get; init; }

    /// <summary>
    /// Total amount in cart after operation
    /// </summary>
    public decimal TotalAmount { get; init; }
}
