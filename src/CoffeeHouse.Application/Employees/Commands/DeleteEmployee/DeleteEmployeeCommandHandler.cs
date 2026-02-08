using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoffeeHouse.Application.Employees.Commands.DeleteEmployee;

/// <summary>
/// Handler for DeleteEmployeeCommand with proper cascade delete handling
/// </summary>
public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteEmployeeCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of DeleteEmployeeCommandHandler
    /// </summary>
    /// <param name="unitOfWork">Unit of work for data access</param>
    /// <param name="logger">Logger instance</param>
    public DeleteEmployeeCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteEmployeeCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Handles the DeleteEmployeeCommand with proper transaction and cascade delete
    /// </summary>
    /// <param name="request">The command request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result</returns>
    public async Task<Result> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting employee with ID: {EmployeeId}", request.Id);

        // Begin transaction for cascade delete
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Get existing employee
            var employee = await _unitOfWork.Employees.GetByIdAsync(request.Id, cancellationToken);
            if (employee == null)
            {
                _logger.LogWarning("Employee with ID {EmployeeId} not found", request.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure($"Employee with ID {request.Id} not found");
            }

            _logger.LogInformation(
                "Found employee {FullName} (ID: {EmployeeId}). Proceeding with cascade delete.",
                employee.FullName,
                employee.EmployeeId);

            // Delete employee
            _unitOfWork.Employees.Delete(employee);

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Commit transaction
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "Employee with ID {EmployeeId} and all related records deleted successfully",
                request.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error deleting employee with ID {EmployeeId}. Rolling back transaction.",
                request.Id);
            
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            
            return Result.Failure($"Failed to delete employee with ID {request.Id}. Error: {ex.Message}");
        }
    }
}
