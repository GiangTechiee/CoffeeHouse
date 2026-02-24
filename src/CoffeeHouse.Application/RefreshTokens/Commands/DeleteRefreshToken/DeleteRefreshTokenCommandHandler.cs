using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.RefreshTokens.Commands.DeleteRefreshToken;

public class DeleteRefreshTokenCommandHandler : IRequestHandler<DeleteRefreshTokenCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRefreshTokenCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await _unitOfWork.RefreshTokens.GetByIdAsync(request.Id, cancellationToken);
        if (token == null)
        {
            throw new NotFoundException($"RefreshToken with ID {request.Id} not found");
        }

        _unitOfWork.RefreshTokens.Delete(token);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
