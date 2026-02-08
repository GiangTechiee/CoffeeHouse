using CoffeeHouse.Application.Carts.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Carts.Queries.GetCart;

public class GetCartQuery : IRequest<CartDto>
{
    public int CustomerId { get; set; }
}
