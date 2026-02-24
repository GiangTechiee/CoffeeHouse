using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Suppliers.Commands.CreateSupplier;
using CoffeeHouse.Application.Suppliers.Commands.DeleteSupplier;
using CoffeeHouse.Application.Suppliers.Commands.UpdateSupplier;
using CoffeeHouse.Application.Suppliers.DTOs;
using CoffeeHouse.Application.Suppliers.Queries.GetSupplierById;
using CoffeeHouse.Application.Suppliers.Queries.GetSuppliers;
using CoffeeHouse.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Controllers.API.Admin;

/// <summary>
/// RESTful API for Supplier management (Admin)
/// </summary>
[Route("api/v1/admin/suppliers")]
[ApiController]
[Authorize(Roles = "Admin,Employee")]
[Produces("application/json")]
public class AdminSuppliersApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminSuppliersApiController> _logger;

    public AdminSuppliersApiController(IMediator mediator, ILogger<AdminSuppliersApiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<SupplierDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSuppliers([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
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

            var result = await _mediator.Send(new GetSuppliersQuery(page, pageSize, search));
            var response = new PaginatedResponse<SupplierDto>(
                result.Items.ToList(),
                result.TotalCount,
                result.PageNumber,
                result.PageSize);

            return Ok(ApiResponse<PaginatedResponse<SupplierDto>>.SuccessResult(
                response,
                new ApiMetadata { RequestId = HttpContext.TraceIdentifier }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving suppliers");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SupplierDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSupplier(int id)
    {
        try
        {
            var supplier = await _mediator.Send(new GetSupplierByIdQuery(id));
            return Ok(ApiResponse<SupplierDto>.SuccessResult(supplier, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("SUPPLIER_NOT_FOUND", $"Supplier with ID {id} was not found", new { supplierId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving supplier {SupplierId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<SupplierDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            var dto = await _mediator.Send(new CreateSupplierCommand(
                request.SupplierName,
                request.PhoneNumber,
                request.Address,
                request.Stk,
                request.Status));

            return CreatedAtAction(nameof(GetSupplier), new { id = dto.SupplierId },
                ApiResponse<SupplierDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating supplier");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<SupplierDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateSupplier(int id, [FromBody] UpdateSupplierRequest request)
    {
        try
        {
            if (id != request.SupplierId)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("ID_MISMATCH", "Supplier ID in URL does not match ID in request body"));
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            var dto = await _mediator.Send(new UpdateSupplierCommand(
                request.SupplierId,
                request.SupplierName,
                request.PhoneNumber,
                request.Address,
                request.Stk,
                request.Status));

            return Ok(ApiResponse<SupplierDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("SUPPLIER_NOT_FOUND", $"Supplier with ID {id} was not found", new { supplierId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating supplier {SupplierId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSupplier(int id)
    {
        try
        {
            await _mediator.Send(new DeleteSupplierCommand(id));

            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("SUPPLIER_NOT_FOUND", $"Supplier with ID {id} was not found", new { supplierId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting supplier {SupplierId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }
}

public class CreateSupplierRequest
{
    [Required]
    [StringLength(255)]
    public string SupplierName { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Address { get; set; }

    [Required]
    [StringLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Stk { get; set; }

    [StringLength(20)]
    public string? Status { get; set; }
}

public class UpdateSupplierRequest : CreateSupplierRequest
{
    [Required]
    public int SupplierId { get; set; }
}
