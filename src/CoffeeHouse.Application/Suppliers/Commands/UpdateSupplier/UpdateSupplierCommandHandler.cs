using CoffeeHouse.Application.Suppliers.DTOs;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.Suppliers.Commands.UpdateSupplier;

public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, SupplierDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSupplierCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<SupplierDto> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier == null)
        {
            throw new NotFoundException($"Supplier with ID {request.SupplierId} not found");
        }

        supplier.SupplierName = request.SupplierName.Trim();
        supplier.PhoneNumber = request.PhoneNumber.Trim();
        supplier.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();
        supplier.Stk = string.IsNullOrWhiteSpace(request.Stk) ? null : request.Stk.Trim();
        supplier.Status = string.IsNullOrWhiteSpace(request.Status) ? supplier.Status : request.Status.Trim();
        supplier.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Suppliers.Update(supplier);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SupplierDto
        {
            SupplierId = supplier.SupplierId,
            SupplierName = supplier.SupplierName,
            Address = supplier.Address,
            PhoneNumber = supplier.PhoneNumber,
            Stk = supplier.Stk,
            Status = supplier.Status,
            CreatedAt = supplier.CreatedAt,
            UpdatedAt = supplier.UpdatedAt
        };
    }
}
