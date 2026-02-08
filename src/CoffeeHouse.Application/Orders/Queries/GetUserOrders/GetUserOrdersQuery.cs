using CoffeeHouse.Application.Orders.DTOs;
using MediatR;
using System.Collections.Generic;

namespace CoffeeHouse.Application.Orders.Queries.GetUserOrders;

public class GetUserOrdersQuery : IRequest<List<OrderDto>>
{
    public int CustomerId { get; set; }
}
