using CoffeeHouse.Application.Carts.DTOs;
using CoffeeHouse.Domain.Interfaces;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoffeeHouse.Application.Carts.Queries.GetCart;

public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCartQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var cartItems = await _unitOfWork.Carts.GetCartItemsAsync(request.CustomerId, cancellationToken);
        
        return new CartDto
        {
            Items = cartItems.Select(ci => new CartItemDto
            {
                ProductId = ci.ProductId,
                ProductName = ci.Product?.ProductName ?? "Unknown",
                Price = ci.Product?.Price ?? 0,
                Quantity = ci.Quantity,
                ImageUrl = ci.Product?.ImageUrl
            }).ToList()
        };
    }
}
