using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Orders.Commands.UpdateAdminOrderStatus;
using CoffeeHouse.Application.Orders.DTOs;
using CoffeeHouse.Application.Orders.Queries.GetAdminSalesOrderById;
using CoffeeHouse.Application.Orders.Queries.GetAdminSalesOrders;
using CoffeeHouse.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Controllers.API.Admin;

/// <summary>
/// RESTful API for Sales Order management (Admin)
/// </summary>
[Route("api/v1/admin/sales-orders")]
[ApiController]
[Authorize(Roles = "Admin,Employee")]
[Produces("application/json")]
public class AdminSalesOrdersApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminSalesOrdersApiController> _logger;

    public AdminSalesOrdersApiController(IMediator mediator, ILogger<AdminSalesOrdersApiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<AdminSalesOrderListItemDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSalesOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchDate = null)
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

            DateTime? dateFilter = null;
            if (!string.IsNullOrWhiteSpace(searchDate))
            {
                if (!DateTime.TryParse(searchDate, out var parsedDate))
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("INVALID_DATE", "searchDate must be a valid date"));
                }
                dateFilter = parsedDate.Date;
            }

            var result = await _mediator.Send(new GetAdminSalesOrdersQuery(page, pageSize, dateFilter));
            var response = new PaginatedResponse<AdminSalesOrderListItemDto>(
                result.Items.ToList(),
                result.TotalCount,
                result.PageNumber,
                result.PageSize);

            return Ok(ApiResponse<PaginatedResponse<AdminSalesOrderListItemDto>>.SuccessResult(
                response,
                new ApiMetadata { RequestId = HttpContext.TraceIdentifier }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving sales orders");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AdminSalesOrderDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSalesOrder(Guid id)
    {
        try
        {
            var dto = await _mediator.Send(new GetAdminSalesOrderByIdQuery(id));
            return Ok(ApiResponse<AdminSalesOrderDetailDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("ORDER_NOT_FOUND", "Sales order not found", new { orderId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving sales order {OrderId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin,Employee")]
    [ProducesResponseType(typeof(ApiResponse<AdminSalesOrderDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateSalesOrderStatus(Guid id, [FromBody] UpdateSalesOrderStatusRequest request)
    {
        try
        {
            if (id != request.OrderId)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("ID_MISMATCH", "Order ID in URL does not match ID in request body"));
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            var dto = await _mediator.Send(new UpdateAdminOrderStatusCommand(
                request.OrderId,
                request.Status,
                request.EmployeeId));

            return Ok(ApiResponse<AdminSalesOrderDetailDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (NotFoundException ex)
        {
            var code = ex.Message.Contains("Employee", StringComparison.OrdinalIgnoreCase)
                ? "EMPLOYEE_NOT_FOUND"
                : "ORDER_NOT_FOUND";

            object details = code == "EMPLOYEE_NOT_FOUND"
                ? new { employeeId = request.EmployeeId }
                : new { orderId = id };

            return NotFound(ApiResponse<object>.ErrorResult(code, ex.Message, details));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating sales order status {OrderId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }
}

public class UpdateSalesOrderStatusRequest
{
    [Required]
    public Guid OrderId { get; set; }

    public string? Status { get; set; }
    public int? EmployeeId { get; set; }
}
