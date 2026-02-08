using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Orders.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Orders.Queries.GetOrders;

/// <summary>
/// Query to get a list of orders with optional filters
/// </summary>
public record GetOrdersQuery : IRequest<PagedResult<OrderDto>>
{
    /// <summary>
    /// Optional customer ID to filter orders
    /// </summary>
    public int? CustomerId { get; init; }

    /// <summary>
    /// Optional employee ID to filter orders
    /// </summary>
    public int? EmployeeId { get; init; }

    /// <summary>
    /// Optional coffee shop ID to filter orders
    /// </summary>
    public int? CoffeeShopId { get; init; }

    /// <summary>
    /// Optional status to filter orders (e.g., "Pending", "Confirmed", "Completed", "Cancelled")
    /// </summary>
    public string? Status { get; init; }

    /// <summary>
    /// Optional start date to filter orders
    /// </summary>
    public DateTime? StartDate { get; init; }

    /// <summary>
    /// Optional end date to filter orders
    /// </summary>
    public DateTime? EndDate { get; init; }

    /// <summary>
    /// Page number (1-based). Default is 1.
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Number of items per page. Default is 10.
    /// </summary>
    public int PageSize { get; init; } = 10;
}
