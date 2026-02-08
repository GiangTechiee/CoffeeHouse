using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Employees.DTOs;
using CoffeeHouse.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoffeeHouse.Application.Employees.Queries.GetEmployees;

/// <summary>
/// Handler for GetEmployeesQuery - Optimized with projection for list views
/// </summary>
public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, PagedResult<EmployeeDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetEmployeesQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of GetEmployeesQueryHandler
    /// </summary>
    /// <param name="unitOfWork">Unit of work for data access</param>
    /// <param name="logger">Logger instance</param>
    public GetEmployeesQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetEmployeesQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetEmployeesQuery
    /// </summary>
    /// <param name="request">The query request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of EmployeeDto</returns>
    public async Task<PagedResult<EmployeeDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting employees - Page: {PageNumber}, PageSize: {PageSize}, CoffeeShopId: {CoffeeShopId}, SearchTerm: {SearchTerm}",
            request.PageNumber,
            request.PageSize,
            request.CoffeeShopId,
            request.SearchTerm);

        // Build specification
        var spec = new Specifications.EmployeeSpecification();

        if (request.CoffeeShopId.HasValue)
        {
            spec.ByCoffeeShop(request.CoffeeShopId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Position))
        {
            spec.ByPosition(request.Position);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            spec.SearchByNameOrPhone(request.SearchTerm);
        }

        if (request.MinSalary.HasValue || request.MaxSalary.HasValue)
        {
            spec.BySalaryRange(request.MinSalary, request.MaxSalary);
        }

        // Default ordering by full name
        spec.OrderByFullName();

        // Execute query with specification and pagination
        var (items, totalCount) = await _unitOfWork.Employees.GetWithSpecificationAsync(
            query => spec.Apply(query),
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        // Calculate total pages
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        // Map to DTOs
        var employeeDtos = items.Select(MapToDto).ToList();

        _logger.LogInformation(
            "Retrieved {Count} employees out of {TotalCount} total",
            employeeDtos.Count,
            totalCount);

        return new PagedResult<EmployeeDto>
        {
            Items = employeeDtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
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
