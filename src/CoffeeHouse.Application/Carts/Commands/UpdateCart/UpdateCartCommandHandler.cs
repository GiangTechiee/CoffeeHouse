using CoffeeHouse.Domain.Interfaces;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoffeeHouse.Application.Carts.Commands.UpdateCart;

public class UpdateCartCommandHandler : IRequestHandler<UpdateCartCommand, UpdateCartResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCartCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateCartResult> Handle(UpdateCartCommand request, CancellationToken cancellationToken)
    {
        foreach (var update in request.Updates)
        {
            var cartItem = await _unitOfWork.Carts.GetCartItemAsync(request.CustomerId, update.ProductId, cancellationToken);
            if (cartItem != null)
            {
                cartItem.Quantity = update.Quantity;
                _unitOfWork.Carts.Update(cartItem);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var cartItems = await _unitOfWork.Carts.GetCartItemsAsync(request.CustomerId, cancellationToken);
        
        return new UpdateCartResult
        {
            Success = true,
            Message = "Cart updated successfully",
            TotalAmount = cartItems.Sum(i => i.Quantity * (i.Product?.Price ?? 0)),
            TotalItems = cartItems.Sum(i => i.Quantity)
        };
    }
}
