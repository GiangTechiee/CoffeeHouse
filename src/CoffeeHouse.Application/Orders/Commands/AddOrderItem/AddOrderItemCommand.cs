using CoffeeHouse.Application.Orders.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Orders.Commands.AddOrderItem;

/// <summary>
/// Command to add an item to an existing order
/// </summary>
public record AddOrderItemCommand : IRequest<OrderDto>
{
    /// <summary>
    /// Order ID to add the item to
    /// </summary>
    public Guid OrderId { get; init; }

    /// <summary>
    /// Product ID to add
    /// </summary>
    public int ProductId { get; init; }

    /// <summary>
    /// Quantity to add
    /// </summary>
    public int Quantity { get; init; }
}
