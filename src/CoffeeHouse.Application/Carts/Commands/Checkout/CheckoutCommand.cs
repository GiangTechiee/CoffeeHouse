using MediatR;

namespace CoffeeHouse.Application.Carts.Commands.Checkout;

/// <summary>
/// Command to checkout and create an order from cart items
/// </summary>
public record CheckoutCommand : IRequest<CheckoutResult>
{
    /// <summary>
    /// Customer ID
    /// </summary>
    public int CustomerId { get; init; }

    /// <summary>
    /// Customer name
    /// </summary>
    public string CustomerName { get; init; } = string.Empty;

    /// <summary>
    /// Phone number
    /// </summary>
    public string PhoneNumber { get; init; } = string.Empty;

    /// <summary>
    /// Delivery address
    /// </summary>
    public string Address { get; init; } = string.Empty;

    /// <summary>
    /// Coffee shop ID (MaQuan)
    /// </summary>
    public int CoffeeShopId { get; init; }

    /// <summary>
    /// Payment method
    /// </summary>
    public string PaymentMethod { get; init; } = string.Empty;

    /// <summary>
    /// Employee ID (optional)
    /// </summary>
    public int? EmployeeId { get; init; }
}

/// <summary>
/// Result of checkout operation
/// </summary>
public record CheckoutResult
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
    /// Order ID (MaHoaDon) created from checkout
    /// </summary>
    public Guid OrderId { get; init; }

    /// <summary>
    /// Total amount of the order
    /// </summary>
    public decimal TotalAmount { get; init; }
}
