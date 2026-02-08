using CoffeeHouse.Application.Employees.DTOs;
using CoffeeHouse.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoffeeHouse.Application.Employees.Queries.GetEmployeeById;

/// <summary>
/// Handler for GetEmployeeByIdQuery
/// </summary>
public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetEmployeeByIdQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of GetEmployeeByIdQueryHandler
    /// </summary>
    /// <param name="unitOfWork">Unit of work for data access</param>
    /// <param name="logger">Logger instance</param>
    public GetEmployeeByIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetEmployeeByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetEmployeeByIdQuery
    /// </summary>
    /// <param name="request">The query request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>EmployeeDto if found, null otherwise</returns>
    public async Task<EmployeeDto?> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting employee by ID: {EmployeeId}, IncludeOrders: {IncludeOrders}",
            request.Id,
            request.IncludeOrders);

        Domain.Entities.Employee? employee;

        if (request.IncludeOrders)
        {
            employee = await _unitOfWork.Employees.GetWithOrdersAsync(request.Id, cancellationToken);
        }
        else
        {
            employee = await _unitOfWork.Employees.GetByIdAsync(request.Id, cancellationToken);
        }

        if (employee == null)
        {
            _logger.LogWarning("Employee with ID {EmployeeId} not found", request.Id);
            return null;
        }

        _logger.LogInformation("Employee with ID {EmployeeId} retrieved successfully", request.Id);

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
            TotalSalary = employee.BaseSalary * employee.SalaryCoefficient, // Calculate
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt
        };
    }
}
