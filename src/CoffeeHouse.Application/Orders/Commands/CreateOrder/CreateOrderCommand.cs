using CoffeeHouse.Application.Orders.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Orders.Commands.CreateOrder;

/// <summary>
/// Command to create a new order
/// </summary>
public record CreateOrderCommand : IRequest<OrderDto>
{
    /// <summary>
    /// Coffee shop ID where order is placed
    /// </summary>
    public int CoffeeShopId { get; init; }

    /// <summary>
    /// Customer ID who is placing the order
    /// </summary>
    public int CustomerId { get; init; }

    /// <summary>
    /// Payment method for the order
    /// </summary>
    public string PaymentMethod { get; init; } = string.Empty;

    /// <summary>
    /// Employee ID who is processing the order (optional)
    /// </summary>
    public int? EmployeeId { get; init; }

    /// <summary>
    /// List of items to add to the order
    /// </summary>
    public List<CreateOrderItemDto> Items { get; init; } = new();
}

/// <summary>
/// DTO for creating an order item
/// </summary>
public record CreateOrderItemDto
{
    /// <summary>
    /// Product ID
    /// </summary>
    public int ProductId { get; init; }

    /// <summary>
    /// Product name
    /// </summary>
    public string ProductName { get; init; } = string.Empty;

    /// <summary>
    /// Quantity to order
    /// </summary>
    public int Quantity { get; init; }

    /// <summary>
    /// Unit price of the product
    /// </summary>
    public decimal UnitPrice { get; init; }
}
