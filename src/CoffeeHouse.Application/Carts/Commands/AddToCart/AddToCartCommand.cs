using MediatR;

namespace CoffeeHouse.Application.Carts.Commands.AddToCart;

/// <summary>
/// Command to add a product to the shopping cart
/// </summary>
public record AddToCartCommand : IRequest<AddToCartResult>
{
    /// <summary>
    /// Customer ID
    /// </summary>
    public int CustomerId { get; init; }

    /// <summary>
    /// Product ID to add to cart
    /// </summary>
    public int ProductId { get; init; }

    /// <summary>
    /// Quantity to add
    /// </summary>
    public int Quantity { get; init; }
}

/// <summary>
/// Result of adding item to cart
/// </summary>
public record AddToCartResult
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
