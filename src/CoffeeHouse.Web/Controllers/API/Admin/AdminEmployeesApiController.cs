using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Employees.Commands.CreateEmployee;
using CoffeeHouse.Application.Employees.Commands.DeleteEmployee;
using CoffeeHouse.Application.Employees.Commands.UpdateEmployee;
using CoffeeHouse.Application.Employees.DTOs;
using CoffeeHouse.Application.Employees.Queries.GetEmployeeById;
using CoffeeHouse.Application.Employees.Queries.GetEmployees;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeHouse.Controllers.API.Admin;

/// <summary>
/// RESTful API for Employee management (Admin)
/// </summary>
[Route("api/v1/admin/employees")]
[ApiController]
[Authorize(Roles = "Admin,Employee")]
[Produces("application/json")]
public class AdminEmployeesApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminEmployeesApiController> _logger;

    public AdminEmployeesApiController(IMediator mediator, ILogger<AdminEmployeesApiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<EmployeeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmployees(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] int? storeId = null,
        [FromQuery] string? position = null)
    {
        try
        {
            if (page < 1)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("INVALID_PAGE", "Page number must be greater than 0"));
            }

            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("INVALID_PAGE_SIZE", "Page size must be between 1 and 100"));
            }

            var query = new GetEmployeesQuery
            {
                PageNumber = page,
                PageSize = pageSize,
                SearchTerm = search,
                CoffeeShopId = storeId,
                Position = position
            };

            var result = await _mediator.Send(query);

            var response = new PaginatedResponse<EmployeeDto>(result.Items.ToList(), result.TotalCount, page, pageSize);

            return Ok(ApiResponse<PaginatedResponse<EmployeeDto>>.SuccessResult(
                response,
                new ApiMetadata { RequestId = HttpContext.TraceIdentifier }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving employees");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmployee(int id)
    {
        try
        {
            var query = new GetEmployeeByIdQuery { Id = id };
            var employee = await _mediator.Send(query);

            if (employee == null)
            {
                return NotFound(ApiResponse<object>.ErrorResult("EMPLOYEE_NOT_FOUND", $"Employee with ID {id} was not found", new { employeeId = id }));
            }

            return Ok(ApiResponse<EmployeeDto>.SuccessResult(employee, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving employee {EmployeeId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeCommand command)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            var result = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetEmployee), new { id = result.Id },
                ApiResponse<EmployeeDto>.SuccessResult(result, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating employee");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("ID_MISMATCH", "Employee ID in URL does not match ID in request body"));
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            var result = await _mediator.Send(command);

            return Ok(ApiResponse<EmployeeDto>.SuccessResult(result, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (CoffeeHouse.Domain.Exceptions.NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("EMPLOYEE_NOT_FOUND", $"Employee with ID {id} was not found", new { employeeId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating employee {EmployeeId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteEmployeeCommand(id));
            if (!result.IsSuccess)
            {
                return NotFound(ApiResponse<object>.ErrorResult("EMPLOYEE_NOT_FOUND", result.Error ?? $"Employee with ID {id} was not found", new { employeeId = id }));
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting employee {EmployeeId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }
}
