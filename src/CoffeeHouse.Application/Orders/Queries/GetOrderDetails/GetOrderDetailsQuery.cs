using CoffeeHouse.Application.Orders.DTOs;
using MediatR;
using System;

namespace CoffeeHouse.Application.Orders.Queries.GetOrderDetails;

public class GetOrderDetailsQuery : IRequest<OrderDto?>
{
    public Guid OrderId { get; set; }
    public int CustomerId { get; set; }
}
