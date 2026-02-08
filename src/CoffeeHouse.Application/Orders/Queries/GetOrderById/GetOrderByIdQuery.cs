using CoffeeHouse.Application.Orders.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Orders.Queries.GetOrderById;

/// <summary>
/// Query to get a single order by its ID
/// </summary>
public record GetOrderByIdQuery : IRequest<OrderDto>
{
    /// <summary>
    /// Order ID to retrieve
    /// </summary>
    public Guid OrderId { get; init; }

    /// <summary>
    /// Whether to include order items in the result. Default is true.
    /// </summary>
    public bool IncludeItems { get; init; } = true;
}
