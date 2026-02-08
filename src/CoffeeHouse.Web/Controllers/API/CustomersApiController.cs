using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Customers.Commands.CreateCustomer;
using CoffeeHouse.Application.Customers.Commands.UpdateCustomer;
using CoffeeHouse.Application.Customers.Commands.DeleteCustomer;
using CoffeeHouse.Application.Customers.DTOs;
using CoffeeHouse.Application.Customers.Queries.GetCustomerById;
using CoffeeHouse.Application.Customers.Queries.GetCustomers;
using CoffeeHouse.Application.Orders.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CoffeeHouse.Controllers.API;

/// <summary>
/// RESTful API for Customer management
/// </summary>
[Route("api/v1/customers")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class CustomersApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CustomersApiController> _logger;

    public CustomersApiController(IMediator mediator, ILogger<CustomersApiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all customers with pagination and filtering
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Employee")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<CustomerDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCustomers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null)
    {
        try
        {
            if (page < 1) return BadRequest(ApiResponse<object>.ErrorResult("INVALID_PAGE", "Page number must be greater than 0"));
            if (pageSize < 1 || pageSize > 100) return BadRequest(ApiResponse<object>.ErrorResult("INVALID_PAGE_SIZE", "Page size must be between 1 and 100"));

            var query = new GetCustomersQuery(page, pageSize, search);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("QUERY_FAILED", result.Error));
            }

            return Ok(ApiResponse<PaginatedResponse<CustomerDto>>.SuccessResult(
                result.Value,
                new ApiMetadata { RequestId = HttpContext.TraceIdentifier }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving customers");
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CustomerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomer(int id)
    {
        try
        {
             // Security: Admin/Employee or Self
            if (!User.IsInRole("Admin") && !User.IsInRole("Employee"))
            {
                var customerIdClaim = User.FindFirst("CustomerId");
                if (customerIdClaim == null || !int.TryParse(customerIdClaim.Value, out int cid) || cid != id)
                {
                    // If user is not staff and requesting someone else's profile, return Forbidden or NotFound
                    // NotFound is safer to avoid enumeration
                    return NotFound(ApiResponse<object>.ErrorResult("CUSTOMER_NOT_FOUND", $"Customer with ID {id} was not found"));
                }
            }

            var query = new GetCustomerByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                 return NotFound(ApiResponse<object>.ErrorResult("CUSTOMER_NOT_FOUND", result.Error ?? $"Customer with ID {id} was not found", new { customerId = id }));
            }

            return Ok(ApiResponse<CustomerDto>.SuccessResult(result.Value, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving customer {CustomerId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpGet("{id}/orders")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<OrderDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomerOrders(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
             // Security: Admin/Employee or Self
            if (!User.IsInRole("Admin") && !User.IsInRole("Employee"))
            {
                var customerIdClaim = User.FindFirst("CustomerId");
                if (customerIdClaim == null || !int.TryParse(customerIdClaim.Value, out int cid) || cid != id)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("CUSTOMER_NOT_FOUND", "Customer not found"));
                }
            }

            // Verify customer exists
            var customerQuery = new GetCustomerByIdQuery { Id = id };
            var customerResult = await _mediator.Send(customerQuery);
            if (!customerResult.IsSuccess) return NotFound(ApiResponse<object>.ErrorResult("CUSTOMER_NOT_FOUND", "Customer not found"));

            // Get customer's orders
            var ordersQuery = new Application.Orders.Queries.GetOrders.GetOrdersQuery
            {
                CustomerId = id,
                PageNumber = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(ordersQuery);
            
            var paginatedResponse = new PaginatedResponse<Application.Orders.DTOs.OrderDto>(
                result.Items.ToList(),
                result.TotalCount,
                page,
                pageSize
            );

            return Ok(ApiResponse<PaginatedResponse<Application.Orders.DTOs.OrderDto>>.SuccessResult(paginatedResponse, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving orders for customer {CustomerId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Employee")] // Administrative creation
    [ProducesResponseType(typeof(ApiResponse<CustomerDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerCommand command)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            var customerId = await _mediator.Send(command);
            
            // Get the created customer to return it
            var getQuery = new GetCustomerByIdQuery { Id = customerId };
            var getResult = await _mediator.Send(getQuery);

            return CreatedAtAction(nameof(GetCustomer), new { id = customerId }, ApiResponse<CustomerDto>.SuccessResult(getResult.Value, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating customer");
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.ErrorResult("INTERNAL_ERROR", ex.Message));
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CustomerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerCommand command)
    {
        try
        {
             // Security: Admin/Employee or Self
            if (!User.IsInRole("Admin") && !User.IsInRole("Employee"))
            {
                var customerIdClaim = User.FindFirst("CustomerId");
                if (customerIdClaim == null || !int.TryParse(customerIdClaim.Value, out int cid) || cid != id)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("CUSTOMER_NOT_FOUND", "Updated customer not found"));
                }
            }

            if (id != command.Id) return BadRequest(ApiResponse<object>.ErrorResult("ID_MISMATCH", "Customer ID in URL does not match ID in request body"));
            if (!ModelState.IsValid) return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));

            await _mediator.Send(command);
            
            // Get the updated customer
            var getQuery = new GetCustomerByIdQuery { Id = id };
            var getResult = await _mediator.Send(getQuery);

            if (!getResult.IsSuccess) return NotFound(ApiResponse<object>.ErrorResult("CUSTOMER_NOT_FOUND", "Updated customer not found"));

            return Ok(ApiResponse<CustomerDto>.SuccessResult(getResult.Value, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating customer {CustomerId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.ErrorResult("INTERNAL_ERROR", ex.Message));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] // Only Admin can delete customers
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        try
        {
            var command = new DeleteCustomerCommand(id);
            var result = await _mediator.Send(command);
            if (!result.IsSuccess) return NotFound(ApiResponse<object>.ErrorResult("DELETE_FAILED", result.Error ?? "Customer not found"));

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting customer {CustomerId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.ErrorResult("INTERNAL_ERROR", ex.Message));
        }
    }
}
