using MediatR;

namespace CoffeeHouse.Application.Carts.Queries.GetCartCount;

public class GetCartCountQuery : IRequest<int>
{
    public int CustomerId { get; set; }
}
