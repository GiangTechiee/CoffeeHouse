using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Customers.Commands.CreateCustomer;
using CoffeeHouse.Application.Customers.Commands.DeleteCustomer;
using CoffeeHouse.Application.Customers.Commands.UpdateCustomer;
using CoffeeHouse.Application.Customers.DTOs;
using CoffeeHouse.Application.Customers.Queries.GetCustomerById;
using CoffeeHouse.Application.Customers.Queries.GetCustomers;
using CoffeeHouse.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Controllers.API.Admin;

/// <summary>
/// RESTful API for Customer management (Admin)
/// </summary>
[Route("api/v1/admin/customers")]
[ApiController]
[Authorize(Roles = "Admin,Employee")]
[Produces("application/json")]
public class AdminCustomersApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminCustomersApiController> _logger;

    public AdminCustomersApiController(IMediator mediator, ILogger<AdminCustomersApiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<CustomerDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCustomers([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
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

            var result = await _mediator.Send(new GetCustomersQuery(page, pageSize, search));
            if (!result.IsSuccess || result.Value == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.ErrorResult("INTERNAL_ERROR", result.Error ?? "Failed to retrieve customers"));
            }

            return Ok(ApiResponse<PaginatedResponse<CustomerDto>>.SuccessResult(
                result.Value,
                new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving customers");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CustomerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomer(int id)
    {
        try
        {
            var result = await _mediator.Send(new GetCustomerByIdQuery { Id = id });
            if (!result.IsSuccess || result.Value == null)
            {
                return NotFound(ApiResponse<object>.ErrorResult("CUSTOMER_NOT_FOUND", "Customer not found", new { customerId = id }));
            }

            return Ok(ApiResponse<CustomerDto>.SuccessResult(result.Value, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving customer {CustomerId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CustomerDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            var id = await _mediator.Send(new CreateCustomerCommand(request.Name, request.PhoneNumber, request.Address));
            var result = await _mediator.Send(new GetCustomerByIdQuery { Id = id });
            if (!result.IsSuccess || result.Value == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.ErrorResult("INTERNAL_ERROR", result.Error ?? "Failed to load created customer"));
            }

            return CreatedAtAction(nameof(GetCustomer), new { id }, ApiResponse<CustomerDto>.SuccessResult(result.Value, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(ApiResponse<object>.ErrorResult("DUPLICATE_PHONE", ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating customer");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CustomerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerRequest request)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("ID_MISMATCH", "Customer ID in URL does not match ID in request body"));
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            await _mediator.Send(new UpdateCustomerCommand(request.Id, request.Name, request.PhoneNumber, request.Address));
            var result = await _mediator.Send(new GetCustomerByIdQuery { Id = id });

            if (!result.IsSuccess || result.Value == null)
            {
                return NotFound(ApiResponse<object>.ErrorResult("CUSTOMER_NOT_FOUND", "Customer not found", new { customerId = id }));
            }

            return Ok(ApiResponse<CustomerDto>.SuccessResult(result.Value, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("CUSTOMER_NOT_FOUND", "Customer not found", new { customerId = id }));
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(ApiResponse<object>.ErrorResult("DUPLICATE_PHONE", ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating customer {CustomerId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteCustomerCommand(id));
            if (!result.IsSuccess)
            {
                return NotFound(ApiResponse<object>.ErrorResult("CUSTOMER_NOT_FOUND", result.Error ?? "Customer not found", new { customerId = id }));
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting customer {CustomerId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }
}

public class CreateCustomerRequest
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Address { get; set; } = string.Empty;
}

public class UpdateCustomerRequest : CreateCustomerRequest
{
    [Required]
    public int Id { get; set; }
}
