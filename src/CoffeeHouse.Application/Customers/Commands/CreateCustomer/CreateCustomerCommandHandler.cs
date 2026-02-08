using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoffeeHouse.Application.Customers.Commands.CreateCustomer;

/// <summary>
/// Handler for CreateCustomerCommand
/// </summary>
public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCustomerCommandHandler> _logger;

    public CreateCustomerCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateCustomerCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<int> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating customer: {Name}", request.Name);

        // Check if customer with phone number already exists
        var exists = await _unitOfWork.Customers.ExistsByPhoneNumberAsync(request.PhoneNumber, null, cancellationToken);
        if (exists)
        {
            _logger.LogWarning("Customer with phone number {PhoneNumber} already exists", request.PhoneNumber);
            throw new InvalidOperationException($"Customer with phone number {request.PhoneNumber} already exists");
        }

        var customer = new Customer
        {
            CustomerName = request.Name,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address
        };

        await _unitOfWork.Customers.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Customer {Name} created successfully with ID {Id}", request.Name, customer.CustomerId);

        return customer.CustomerId;
    }
}
