using CoffeeHouse.Application.Employees.DTOs;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoffeeHouse.Application.Employees.Commands.UpdateEmployee;

/// <summary>
/// Handler for UpdateEmployeeCommand
/// </summary>
public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, EmployeeDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateEmployeeCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of UpdateEmployeeCommandHandler
    /// </summary>
    /// <param name="unitOfWork">Unit of work for data access</param>
    /// <param name="logger">Logger instance</param>
    public UpdateEmployeeCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateEmployeeCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Handles the UpdateEmployeeCommand
    /// </summary>
    /// <param name="request">The command request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>EmployeeDto representing the updated employee</returns>
    public async Task<EmployeeDto> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating employee with ID: {EmployeeId}, name: {FullName}",
            request.Id,
            request.FullName);

        // Get existing employee
        var employee = await _unitOfWork.Employees.GetByIdAsync(request.Id, cancellationToken);
        if (employee == null)
        {
            _logger.LogWarning("Employee with ID {EmployeeId} not found", request.Id);
            throw new NotFoundException($"Employee with ID {request.Id} not found");
        }

        // Check if phone number is being changed and if it's already taken by another employee
        if (employee.PhoneNumber != request.PhoneNumber)
        {
            var existingByPhone = await _unitOfWork.Employees.ExistsByPhoneNumberAsync(
                request.PhoneNumber,
                excludeId: request.Id,
                cancellationToken: cancellationToken);

            if (existingByPhone)
            {
                _logger.LogWarning("Phone number {PhoneNumber} is already taken by another employee", request.PhoneNumber);
                throw new InvalidOperationException($"Phone number '{request.PhoneNumber}' is already taken by another employee");
            }
        }

        // Check if ID card number is being changed and if it's already taken by another employee
        if (employee.IdentityCardNumber != request.IdCardNumber)
        {
            var existingByIdCard = await _unitOfWork.Employees.ExistsByIdCardNumberAsync(
                request.IdCardNumber,
                excludeId: request.Id,
                cancellationToken: cancellationToken);

            if (existingByIdCard)
            {
                _logger.LogWarning("ID card number {IdCardNumber} is already taken by another employee", request.IdCardNumber);
                throw new InvalidOperationException($"ID card number '{request.IdCardNumber}' is already taken by another employee");
            }
        }

        // Update employee properties
        employee.FullName = request.FullName;
        employee.Address = request.Address;
        employee.DateOfBirth = request.DateOfBirth;
        employee.Gender = request.Gender;
        employee.Email = request.Email;
        employee.PhoneNumber = request.PhoneNumber;
        employee.Position = request.Position;
        employee.BaseSalary = request.BaseSalary;
        employee.SalaryCoefficient = request.SalaryCoefficient;
        employee.StoreId = request.CoffeeShopId;
        employee.IdentityCardNumber = request.IdCardNumber;

        // Update in repository
        _unitOfWork.Employees.Update(employee);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Employee with ID {EmployeeId} updated successfully", request.Id);

        // Map to DTO and return
        return MapToDto(employee);
    }

    /// <summary>
    /// Maps Employee entity to EmployeeDto
    /// </summary>
    /// <param name="employee">Employee entity</param>
    /// <returns>EmployeeDto</returns>
    private static EmployeeDto MapToDto(Domain.Entities.Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.EmployeeId,
            CoffeeShopId = employee.StoreId,
            FullName = employee.FullName,
            Address = employee.Address,
            DateOfBirth = employee.DateOfBirth,
            Gender = employee.Gender,
            Position = employee.Position,
            PhoneNumber = employee.PhoneNumber,
            IdCardNumber = employee.IdentityCardNumber,
            Email = employee.Email,
            BaseSalary = employee.BaseSalary,
            SalaryCoefficient = employee.SalaryCoefficient,
            TotalSalary = employee.BaseSalary * employee.SalaryCoefficient,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt
        };
    }
}
