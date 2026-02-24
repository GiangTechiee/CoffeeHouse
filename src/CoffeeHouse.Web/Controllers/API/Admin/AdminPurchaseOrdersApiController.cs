using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.PurchaseOrders.Commands.CreatePurchaseOrder;
using CoffeeHouse.Application.PurchaseOrders.Commands.DeletePurchaseOrder;
using CoffeeHouse.Application.PurchaseOrders.Commands.AddPurchaseOrderItem;
using CoffeeHouse.Application.PurchaseOrders.Commands.UpdatePurchaseOrderItem;
using CoffeeHouse.Application.PurchaseOrders.Commands.RemovePurchaseOrderItem;
using CoffeeHouse.Application.PurchaseOrders.Commands.UpdatePurchaseOrder;
using CoffeeHouse.Application.PurchaseOrders.DTOs;
using CoffeeHouse.Application.PurchaseOrders.Queries.GetPurchaseOrderById;
using CoffeeHouse.Application.PurchaseOrders.Queries.GetPurchaseOrders;
using CoffeeHouse.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Controllers.API.Admin;

/// <summary>
/// RESTful API for Purchase Order management (Admin)
/// </summary>
[Route("api/v1/admin/purchase-orders")]
[ApiController]
[Authorize(Roles = "Admin,Employee")]
[Produces("application/json")]
public class AdminPurchaseOrdersApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminPurchaseOrdersApiController> _logger;

    public AdminPurchaseOrdersApiController(IMediator mediator, ILogger<AdminPurchaseOrdersApiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<PurchaseOrderListItemDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPurchaseOrders(
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

            var result = await _mediator.Send(new GetPurchaseOrdersQuery(page, pageSize, dateFilter));
            var response = new PaginatedResponse<PurchaseOrderListItemDto>(
                result.Items.ToList(),
                result.TotalCount,
                result.PageNumber,
                result.PageSize);

            return Ok(ApiResponse<PaginatedResponse<PurchaseOrderListItemDto>>.SuccessResult(
                response,
                new ApiMetadata { RequestId = HttpContext.TraceIdentifier }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving purchase orders");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PurchaseOrderDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPurchaseOrder(Guid id)
    {
        try
        {
            var dto = await _mediator.Send(new GetPurchaseOrderByIdQuery(id));
            return Ok(ApiResponse<PurchaseOrderDetailDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("PURCHASE_ORDER_NOT_FOUND", $"Purchase order with ID {id} was not found", new { purchaseOrderId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving purchase order {PurchaseOrderId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpGet("{id:guid}/items")]
    [ProducesResponseType(typeof(ApiResponse<List<PurchaseOrderItemDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPurchaseOrderItems(Guid id)
    {
        try
        {
            var dto = await _mediator.Send(new GetPurchaseOrderByIdQuery(id));
            return Ok(ApiResponse<List<PurchaseOrderItemDto>>.SuccessResult(
                dto.Items,
                new ApiMetadata { RequestId = HttpContext.TraceIdentifier }
            ));
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("PURCHASE_ORDER_NOT_FOUND", $"Purchase order with ID {id} was not found", new { purchaseOrderId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving purchase order items {PurchaseOrderId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<PurchaseOrderDetailDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreatePurchaseOrder([FromBody] CreatePurchaseOrderRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            var items = request.Items
                .Where(i => i.Quantity > 0)
                .GroupBy(i => i.IngredientId)
                .Select(g => new PurchaseOrderItemInput
                {
                    IngredientId = g.Key,
                    Quantity = g.Sum(x => x.Quantity),
                    UnitPrice = g.Last().UnitPrice
                })
                .ToList();

            if (items.Count == 0)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("EMPTY_ITEMS", "Purchase order must contain at least one item"));
            }

            var dto = await _mediator.Send(new CreatePurchaseOrderCommand(
                request.StoreId,
                request.EmployeeId,
                request.SupplierId,
                request.OrderDate == default ? null : request.OrderDate,
                request.Description,
                request.Status,
                items));

            return CreatedAtAction(nameof(GetPurchaseOrder), new { id = dto.PurchaseOrderId },
                ApiResponse<PurchaseOrderDetailDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (NotFoundException ex)
        {
            var code = ex.Message.Contains("CafeStore", StringComparison.OrdinalIgnoreCase)
                ? "STORE_NOT_FOUND"
                : ex.Message.Contains("Employee", StringComparison.OrdinalIgnoreCase)
                    ? "EMPLOYEE_NOT_FOUND"
                    : ex.Message.Contains("Supplier", StringComparison.OrdinalIgnoreCase)
                        ? "SUPPLIER_NOT_FOUND"
                        : ex.Message.Contains("Ingredient", StringComparison.OrdinalIgnoreCase)
                            ? "INGREDIENT_NOT_FOUND"
                            : "NOT_FOUND";

            object details = code switch
            {
                "STORE_NOT_FOUND" => new { storeId = request.StoreId },
                "EMPLOYEE_NOT_FOUND" => new { employeeId = request.EmployeeId },
                "SUPPLIER_NOT_FOUND" => new { supplierId = request.SupplierId },
                "INGREDIENT_NOT_FOUND" => new { message = ex.Message },
                _ => new { message = ex.Message }
            };

            return NotFound(ApiResponse<object>.ErrorResult(code, ex.Message, details));
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating purchase order");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<PurchaseOrderDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdatePurchaseOrder(Guid id, [FromBody] UpdatePurchaseOrderRequest request)
    {
        try
        {
            if (id != request.PurchaseOrderId)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("ID_MISMATCH", "PurchaseOrder ID in URL does not match ID in request body"));
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            var dto = await _mediator.Send(new UpdatePurchaseOrderCommand(
                request.PurchaseOrderId,
                request.StoreId,
                request.EmployeeId,
                request.SupplierId,
                request.OrderDate,
                request.Description,
                request.Status));

            return Ok(ApiResponse<PurchaseOrderDetailDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (NotFoundException ex)
        {
            var code = ex.Message.Contains("CafeStore", StringComparison.OrdinalIgnoreCase)
                ? "STORE_NOT_FOUND"
                : ex.Message.Contains("Employee", StringComparison.OrdinalIgnoreCase)
                    ? "EMPLOYEE_NOT_FOUND"
                    : ex.Message.Contains("Supplier", StringComparison.OrdinalIgnoreCase)
                        ? "SUPPLIER_NOT_FOUND"
                        : "PURCHASE_ORDER_NOT_FOUND";

            object details = code switch
            {
                "STORE_NOT_FOUND" => new { storeId = request.StoreId },
                "EMPLOYEE_NOT_FOUND" => new { employeeId = request.EmployeeId },
                "SUPPLIER_NOT_FOUND" => new { supplierId = request.SupplierId },
                _ => new { purchaseOrderId = id }
            };

            return NotFound(ApiResponse<object>.ErrorResult(code, ex.Message, details));
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating purchase order {PurchaseOrderId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePurchaseOrder(Guid id)
    {
        try
        {
            await _mediator.Send(new DeletePurchaseOrderCommand(id));

            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("PURCHASE_ORDER_NOT_FOUND", $"Purchase order with ID {id} was not found", new { purchaseOrderId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting purchase order {PurchaseOrderId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPost("{id:guid}/items")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<PurchaseOrderDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AddPurchaseOrderItem(Guid id, [FromBody] UpsertPurchaseOrderItemRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            var dto = await _mediator.Send(new AddPurchaseOrderItemCommand(
                id,
                request.IngredientId,
                request.Quantity,
                request.UnitPrice));

            return Ok(ApiResponse<PurchaseOrderDetailDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (NotFoundException ex)
        {
            var code = ex.Message.Contains("Ingredient", StringComparison.OrdinalIgnoreCase)
                ? "INGREDIENT_NOT_FOUND"
                : "PURCHASE_ORDER_NOT_FOUND";

            return NotFound(ApiResponse<object>.ErrorResult(code, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding purchase order item {PurchaseOrderId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPut("{id:guid}/items/{ingredientId:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<PurchaseOrderDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdatePurchaseOrderItem(Guid id, int ingredientId, [FromBody] UpsertPurchaseOrderItemRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            if (ingredientId != request.IngredientId)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("ID_MISMATCH", "Ingredient ID in URL does not match ID in request body"));
            }

            var dto = await _mediator.Send(new UpdatePurchaseOrderItemCommand(
                id,
                request.IngredientId,
                request.Quantity,
                request.UnitPrice));

            return Ok(ApiResponse<PurchaseOrderDetailDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (NotFoundException ex)
        {
            var code = ex.Message.Contains("Ingredient", StringComparison.OrdinalIgnoreCase)
                ? "ITEM_NOT_FOUND"
                : "PURCHASE_ORDER_NOT_FOUND";

            return NotFound(ApiResponse<object>.ErrorResult(code, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating purchase order item {PurchaseOrderId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpDelete("{id:guid}/items/{ingredientId:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<PurchaseOrderDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemovePurchaseOrderItem(Guid id, int ingredientId)
    {
        try
        {
            var dto = await _mediator.Send(new RemovePurchaseOrderItemCommand(id, ingredientId));
            return Ok(ApiResponse<PurchaseOrderDetailDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResult("NOT_FOUND", ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while removing purchase order item {PurchaseOrderId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }
}

public class PurchaseOrderItemRequest
{
    [Required]
    public int IngredientId { get; set; }

    [Range(0.001, double.MaxValue)]
    public decimal Quantity { get; set; }

    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }
}

public class UpsertPurchaseOrderItemRequest
{
    [Required]
    public int IngredientId { get; set; }

    [Range(0.001, double.MaxValue)]
    public decimal Quantity { get; set; }

    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }
}

public class CreatePurchaseOrderRequest
{
    [Required]
    public int StoreId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public int SupplierId { get; set; }

    public DateTime OrderDate { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    [Required]
    public List<PurchaseOrderItemRequest> Items { get; set; } = new();
}

public class UpdatePurchaseOrderRequest
{
    [Required]
    public Guid PurchaseOrderId { get; set; }

    [Required]
    public int StoreId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public int SupplierId { get; set; }

    [Required]
    public DateTime OrderDate { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }
}
