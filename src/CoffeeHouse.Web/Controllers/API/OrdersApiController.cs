using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Orders.Commands.CreateOrder;
using CoffeeHouse.Application.Orders.Commands.AddOrderItem;
using CoffeeHouse.Application.Orders.Commands.RemoveOrderItem;
using CoffeeHouse.Application.Orders.Commands.UpdateOrderStatus;
using CoffeeHouse.Application.Orders.DTOs;
using CoffeeHouse.Application.Orders.Queries.GetOrderById;
using CoffeeHouse.Application.Orders.Queries.GetOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Controllers.API;

/// <summary>
/// RESTful API for Order management
/// </summary>
[Route("api/v1/orders")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class OrdersApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersApiController> _logger;

    public OrdersApiController(IMediator mediator, ILogger<OrdersApiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all orders with pagination and filtering
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<OrderDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? customerId = null,
        [FromQuery] string? status = null)
    {
        try
        {
            if (page < 1)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(
                    "INVALID_PAGE",
                    "Page number must be greater than 0"
                ));
            }

            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(
                    "INVALID_PAGE_SIZE",
                    "Page size must be between 1 and 100"
                ));
            }

            // Security: Filter by CustomerId if not staff
            if (!User.IsInRole("Admin") && !User.IsInRole("Manager") && !User.IsInRole("Employee"))
            {
                var customerIdClaim = User.FindFirst("CustomerId");
                if (customerIdClaim != null && int.TryParse(customerIdClaim.Value, out int cid))
                {
                    customerId = cid;
                }
                else
                {
                    // If regular user has no customer ID, they see nothing
                     return Ok(ApiResponse<PaginatedResponse<OrderDto>>.SuccessResult(
                        new PaginatedResponse<OrderDto>(new List<OrderDto>(), 0, page, pageSize),
                        new ApiMetadata { RequestId = HttpContext.TraceIdentifier }
                    ));
                }
            }

            var query = new GetOrdersQuery
            {
                PageNumber = page,
                PageSize = pageSize,
                CustomerId = customerId,
                Status = status
            };

            var result = await _mediator.Send(query);

            var paginatedResponse = new PaginatedResponse<OrderDto>(
                result.Items.ToList(),
                result.TotalCount,
                page,
                pageSize
            );

            return Ok(ApiResponse<PaginatedResponse<OrderDto>>.SuccessResult(
                paginatedResponse,
                new ApiMetadata
                {
                    RequestId = HttpContext.TraceIdentifier
                }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving orders");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult(
                    "INTERNAL_ERROR",
                    "An error occurred while processing your request"
                )
            );
        }
    }

    /// <summary>
    /// Get a specific order by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        try
        {
            var query = new GetOrderByIdQuery { OrderId = id };
            var order = await _mediator.Send(query);

             // Security check: Customer can only see their own order
            if (!User.IsInRole("Admin") && !User.IsInRole("Manager") && !User.IsInRole("Employee"))
            {
                var customerIdClaim = User.FindFirst("CustomerId");
                if (customerIdClaim == null || !int.TryParse(customerIdClaim.Value, out int cid) || order.CustomerId != cid)
                {
                    return NotFound(ApiResponse<object>.ErrorResult(
                        "ORDER_NOT_FOUND",
                        $"Order with ID {id} was not found"
                    ));
                }
            }
            
            return Ok(ApiResponse<OrderDto>.SuccessResult(
                order,
                new ApiMetadata
                {
                    RequestId = HttpContext.TraceIdentifier
                }
            ));
        }
        catch (Domain.Exceptions.NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult(
                "ORDER_NOT_FOUND",
                $"Order with ID {id} was not found",
                new { orderId = id }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving order {OrderId}", id);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult(
                    "INTERNAL_ERROR",
                    "An error occurred while processing your request"
                )
            );
        }
    }

    /// <summary>
    /// Create a new order
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult(
                    "VALIDATION_ERROR",
                    "Request validation failed",
                    ModelState
                ));
            }

            // Security: Force CustomerId for non-staff
            if (!User.IsInRole("Admin") && !User.IsInRole("Manager") && !User.IsInRole("Employee"))
            {
                var customerIdClaim = User.FindFirst("CustomerId");
                if (customerIdClaim != null && int.TryParse(customerIdClaim.Value, out int cid))
                {
                    command = command with { CustomerId = cid };
                }
                else
                {
                    return BadRequest(ApiResponse<object>.ErrorResult(
                        "MISSING_CUSTOMER_PROFILE",
                        "User must have a customer profile to place orders."
                    ));
                }
            }

            var result = await _mediator.Send(command);

            return CreatedAtAction(
                nameof(GetOrder),
                new { id = result.Id },
                ApiResponse<OrderDto>.SuccessResult(
                    result,
                    new ApiMetadata
                    {
                        RequestId = HttpContext.TraceIdentifier
                    }
                )
            );
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error while creating order");
            return UnprocessableEntity(ApiResponse<object>.ErrorResult(
                "VALIDATION_ERROR",
                ex.Message
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating order");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult(
                    "INTERNAL_ERROR",
                    "An error occurred while processing your request"
                )
            );
        }
    }

    /// <summary>
    /// Update order status
    /// </summary>
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin,Manager,Employee")]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusCommand command)
    {
        try
        {
            if (id != command.OrderId)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(
                    "ID_MISMATCH",
                    "Order ID in URL does not match ID in request body"
                ));
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult(
                    "VALIDATION_ERROR",
                    "Request validation failed",
                    ModelState
                ));
            }

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("UPDATE_FAILED", result.Error ?? "Failed to update order status"));
            }

            return Ok(ApiResponse<OrderDto>.SuccessResult(
                result.Value,
                new ApiMetadata
                {
                    RequestId = HttpContext.TraceIdentifier
                }
            ));
        }
        catch (Domain.Exceptions.NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult(
                "ORDER_NOT_FOUND",
                $"Order with ID {id} was not found",
                new { orderId = id }
            ));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error while updating order status {OrderId}", id);
            return UnprocessableEntity(ApiResponse<object>.ErrorResult(
                "VALIDATION_ERROR",
                ex.Message
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating order status {OrderId}", id);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult(
                    "INTERNAL_ERROR",
                    "An error occurred while processing your request"
                )
            );
        }
    }

    /// <summary>
    /// Cancel an order
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CancelOrder(Guid id)
    {
        try
        {
            // Security: Check ownership if not staff
            if (!User.IsInRole("Admin") && !User.IsInRole("Manager") && !User.IsInRole("Employee"))
            {
                var customerIdClaim = User.FindFirst("CustomerId");
                if (customerIdClaim == null || !int.TryParse(customerIdClaim.Value, out int cid))
                {
                     return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<object>.ErrorResult("FORBIDDEN", "Access denied"));
                }

                // Verify order ownership
                try 
                {
                    var query = new GetOrderByIdQuery { OrderId = id };
                    var order = await _mediator.Send(query);
                    
                    if (order.CustomerId != cid)
                    {
                        return NotFound(ApiResponse<object>.ErrorResult("ORDER_NOT_FOUND", $"Order with ID {id} was not found"));
                    }
                }
                catch (Domain.Exceptions.NotFoundException)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("ORDER_NOT_FOUND", $"Order with ID {id} was not found"));
                }
            }

            // Use UpdateOrderStatus to cancel
            var command = new UpdateOrderStatusCommand(id, "Cancelled");
            
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("CANCEL_FAILED", result.Error ?? "Failed to cancel order"));
            }

            return NoContent();
        }
        catch (Domain.Exceptions.NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult(
                "ORDER_NOT_FOUND",
                $"Order with ID {id} was not found",
                new { orderId = id }
            ));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<object>.ErrorResult(
                "INVALID_ORDER_STATE",
                ex.Message,
                new { orderId = id }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while cancelling order {OrderId}", id);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult(
                    "INTERNAL_ERROR",
                    "An error occurred while processing your request"
                )
            );
        }
    }

    /// <summary>
    /// Add an item to an order (pending orders only)
    /// </summary>
    [HttpPost("{id}/items")]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AddOrderItem(Guid id, [FromBody] AddOrderItemRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult(
                    "VALIDATION_ERROR",
                    "Request validation failed",
                    ModelState
                ));
            }

            if (id != request.OrderId)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(
                    "ID_MISMATCH",
                    "Order ID in URL does not match ID in request body"
                ));
            }

            if (!await IsOrderAccessibleAsync(id))
            {
                return NotFound(ApiResponse<object>.ErrorResult(
                    "ORDER_NOT_FOUND",
                    $"Order with ID {id} was not found"
                ));
            }

            var command = new AddOrderItemCommand
            {
                OrderId = request.OrderId,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            };

            var result = await _mediator.Send(command);

            return Ok(ApiResponse<OrderDto>.SuccessResult(
                result,
                new ApiMetadata { RequestId = HttpContext.TraceIdentifier }
            ));
        }
        catch (Domain.Exceptions.NotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResult(
                "NOT_FOUND",
                ex.Message
            ));
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(ApiResponse<object>.ErrorResult(
                "INVALID_ORDER_STATE",
                ex.Message
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding item to order {OrderId}", id);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult(
                    "INTERNAL_ERROR",
                    "An error occurred while processing your request"
                )
            );
        }
    }

    /// <summary>
    /// Remove an item from an order (pending orders only)
    /// </summary>
    [HttpDelete("{id}/items/{productId:int}")]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveOrderItem(Guid id, int productId)
    {
        try
        {
            if (!await IsOrderAccessibleAsync(id))
            {
                return NotFound(ApiResponse<object>.ErrorResult(
                    "ORDER_NOT_FOUND",
                    $"Order with ID {id} was not found"
                ));
            }

            var command = new RemoveOrderItemCommand
            {
                OrderId = id,
                ProductId = productId
            };

            var result = await _mediator.Send(command);

            return Ok(ApiResponse<OrderDto>.SuccessResult(
                result,
                new ApiMetadata { RequestId = HttpContext.TraceIdentifier }
            ));
        }
        catch (Domain.Exceptions.NotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResult(
                "NOT_FOUND",
                ex.Message
            ));
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(ApiResponse<object>.ErrorResult(
                "INVALID_ORDER_STATE",
                ex.Message
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while removing item from order {OrderId}", id);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult(
                    "INTERNAL_ERROR",
                    "An error occurred while processing your request"
                )
            );
        }
    }

    private async Task<bool> IsOrderAccessibleAsync(Guid orderId)
    {
        if (User.IsInRole("Admin") || User.IsInRole("Manager") || User.IsInRole("Employee"))
        {
            return true;
        }

        var customerIdClaim = User.FindFirst("CustomerId");
        if (customerIdClaim == null || !int.TryParse(customerIdClaim.Value, out int cid))
        {
            return false;
        }

        try
        {
            var query = new GetOrderByIdQuery { OrderId = orderId };
            var order = await _mediator.Send(query);
            return order.CustomerId == cid;
        }
        catch (Domain.Exceptions.NotFoundException)
        {
            return false;
        }
    }
}

public class AddOrderItemRequest
{
    [Required]
    public Guid OrderId { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Range(1, 999)]
    public int Quantity { get; set; } = 1;
}
