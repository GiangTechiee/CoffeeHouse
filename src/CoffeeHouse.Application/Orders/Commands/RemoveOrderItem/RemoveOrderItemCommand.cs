using CoffeeHouse.Application.Orders.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Orders.Commands.RemoveOrderItem;

/// <summary>
/// Command to remove an item from an existing order
/// </summary>
public record RemoveOrderItemCommand : IRequest<OrderDto>
{
    /// <summary>
    /// Order ID to remove the item from
    /// </summary>
    public Guid OrderId { get; init; }

    /// <summary>
    /// Product ID to remove
    /// </summary>
    public int ProductId { get; init; }
}
