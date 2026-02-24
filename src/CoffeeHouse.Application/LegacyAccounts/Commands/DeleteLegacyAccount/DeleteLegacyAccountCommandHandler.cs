using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.LegacyAccounts.Commands.DeleteLegacyAccount;

public class DeleteLegacyAccountCommandHandler : IRequestHandler<DeleteLegacyAccountCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteLegacyAccountCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteLegacyAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId, cancellationToken);
        if (account == null)
        {
            throw new NotFoundException($"Account with ID {request.AccountId} not found");
        }

        _unitOfWork.Accounts.Delete(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
