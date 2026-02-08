using CoffeeHouse.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CoffeeHouse.Application.Carts.Queries.GetCartCount;

public class GetCartCountQueryHandler : IRequestHandler<GetCartCountQuery, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCartCountQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(GetCartCountQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Carts.GetCartCountAsync(request.CustomerId, cancellationToken);
    }
}
