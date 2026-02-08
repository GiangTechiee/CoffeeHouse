using CoffeeHouse.Application.Employees.DTOs;
using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoffeeHouse.Application.Employees.Commands.CreateEmployee;

/// <summary>
/// Handler for CreateEmployeeCommand
/// </summary>
public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, EmployeeDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateEmployeeCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of CreateEmployeeCommandHandler
    /// </summary>
    /// <param name="unitOfWork">Unit of work for data access</param>
    /// <param name="logger">Logger instance</param>
    public CreateEmployeeCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateEmployeeCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Handles the CreateEmployeeCommand
    /// </summary>
    /// <param name="request">The command request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>EmployeeDto representing the created employee</returns>
    public async Task<EmployeeDto> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating employee with name: {FullName}, position: {Position}, coffeeShopId: {CoffeeShopId}",
            request.FullName,
            request.Position,
            request.CoffeeShopId);

        // Check if employee with same phone number already exists
        var existingByPhone = await _unitOfWork.Employees.ExistsByPhoneNumberAsync(
            request.PhoneNumber, 
            cancellationToken: cancellationToken);
        
        if (existingByPhone)
        {
            _logger.LogWarning("Employee with phone number {PhoneNumber} already exists", request.PhoneNumber);
            throw new InvalidOperationException($"Employee with phone number '{request.PhoneNumber}' already exists");
        }

        // Check if employee with same ID card number already exists
        var existingByIdCard = await _unitOfWork.Employees.ExistsByIdCardNumberAsync(
            request.IdCardNumber, 
            cancellationToken: cancellationToken);
        
        if (existingByIdCard)
        {
            _logger.LogWarning("Employee with ID card number {IdCardNumber} already exists", request.IdCardNumber);
            throw new InvalidOperationException($"Employee with ID card number '{request.IdCardNumber}' already exists");
        }

        // Create domain entity
        var employee = new Employee
        {
            StoreId = request.CoffeeShopId,
            FullName = request.FullName,
            Position = request.Position,
            PhoneNumber = request.PhoneNumber,
            IdentityCardNumber = request.IdCardNumber,
            BaseSalary = request.BaseSalary,
            SalaryCoefficient = request.SalaryCoefficient,
            Address = request.Address,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Email = request.Email
        };

        // Add employee to repository
        await _unitOfWork.Employees.AddAsync(employee, cancellationToken);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Employee created successfully with ID: {EmployeeId}", employee.EmployeeId);

        // Map to DTO and return
        return MapToDto(employee);
    }

    /// <summary>
    /// Maps Employee entity to EmployeeDto
    /// </summary>
    /// <param name="employee">Employee entity</param>
    /// <returns>EmployeeDto</returns>
    private static EmployeeDto MapToDto(Employee employee)
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
            TotalSalary = employee.BaseSalary * employee.SalaryCoefficient, // TotalSalary is calculated
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt
        };
    }
}
