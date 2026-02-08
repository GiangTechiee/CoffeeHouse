using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoffeeHouse.Application.Customers.Commands.UpdateCustomer;

/// <summary>
/// Handler for UpdateCustomerCommand
/// </summary>
public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateCustomerCommandHandler> _logger;

    public UpdateCustomerCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateCustomerCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating customer with ID {Id}", request.Id);

        var customer = await _unitOfWork.Customers.GetByIdAsync(request.Id, cancellationToken);
        if (customer == null)
        {
            _logger.LogWarning("Customer with ID {Id} not found", request.Id);
            throw new NotFoundException($"Customer with ID {request.Id} not found");
        }

        // Check if phone number is already taken by another customer
        var exists = await _unitOfWork.Customers.ExistsByPhoneNumberAsync(request.PhoneNumber, request.Id, cancellationToken);
        if (exists)
        {
            _logger.LogWarning("Customer with phone number {PhoneNumber} already exists", request.PhoneNumber);
            throw new InvalidOperationException($"Customer with phone number {request.PhoneNumber} already exists");
        }

        customer.CustomerName = request.Name;
        customer.PhoneNumber = request.PhoneNumber;
        customer.Address = request.Address;

        _unitOfWork.Customers.Update(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Customer with ID {Id} updated successfully", request.Id);

        return Unit.Value;
    }
}
