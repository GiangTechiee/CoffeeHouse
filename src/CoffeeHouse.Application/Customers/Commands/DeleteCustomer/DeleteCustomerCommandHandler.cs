using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoffeeHouse.Application.Customers.Commands.DeleteCustomer;

/// <summary>
/// Handler for DeleteCustomerCommand
/// </summary>
public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteCustomerCommandHandler> _logger;

    public DeleteCustomerCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteCustomerCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting customer with ID: {CustomerId}", request.Id);

        var customer = await _unitOfWork.Customers.GetByIdAsync(request.Id, cancellationToken);
        if (customer == null)
        {
             _logger.LogWarning("Customer with ID {CustomerId} not found", request.Id);
             return Result.Failure($"Customer with ID {request.Id} not found");
        }

        _unitOfWork.Customers.Delete(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully deleted customer with ID: {CustomerId}", request.Id);

        return Result.Success();
    }
}
