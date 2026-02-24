using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.Suppliers.Commands.DeleteSupplier;

public record DeleteSupplierCommand(int SupplierId) : IRequest;

public class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSupplierCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier == null)
        {
            throw new NotFoundException($"Supplier with ID {request.SupplierId} not found");
        }

        _unitOfWork.Suppliers.Delete(supplier);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
