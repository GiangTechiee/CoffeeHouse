using CoffeeHouse.Application.CafeStores.Commands.CreateCafeStore;
using CoffeeHouse.Application.CafeStores.Commands.DeleteCafeStore;
using CoffeeHouse.Application.CafeStores.Commands.UpdateCafeStore;
using CoffeeHouse.Application.CafeStores.DTOs;
using CoffeeHouse.Application.CafeStores.Queries.GetCafeStoreById;
using CoffeeHouse.Application.CafeStores.Queries.GetCafeStores;
using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Controllers.API.Admin;

/// <summary>
/// RESTful API for CafeStore management (Admin)
/// </summary>
[Route("api/v1/admin/cafe-stores")]
[ApiController]
[Authorize(Roles = "Admin,Employee")]
[Produces("application/json")]
public class AdminCafeStoresApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminCafeStoresApiController> _logger;

    public AdminCafeStoresApiController(IMediator mediator, ILogger<AdminCafeStoresApiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<CafeStoreDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCafeStores([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
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

            var result = await _mediator.Send(new GetCafeStoresQuery(page, pageSize, search));
            var response = new PaginatedResponse<CafeStoreDto>(
                result.Items.ToList(),
                result.TotalCount,
                result.PageNumber,
                result.PageSize);

            return Ok(ApiResponse<PaginatedResponse<CafeStoreDto>>.SuccessResult(
                response,
                new ApiMetadata { RequestId = HttpContext.TraceIdentifier }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving cafe stores");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CafeStoreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCafeStore(int id)
    {
        try
        {
            var store = await _mediator.Send(new GetCafeStoreByIdQuery(id));
            return Ok(ApiResponse<CafeStoreDto>.SuccessResult(store, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("STORE_NOT_FOUND", $"Cafe store with ID {id} was not found", new { storeId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving cafe store {StoreId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CafeStoreDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateCafeStore([FromBody] CreateCafeStoreRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            var dto = await _mediator.Send(new CreateCafeStoreCommand(
                request.StoreName,
                request.Address,
                request.PhoneNumber,
                request.Email));

            return CreatedAtAction(nameof(GetCafeStore), new { id = dto.StoreId },
                ApiResponse<CafeStoreDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating cafe store");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CafeStoreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateCafeStore(int id, [FromBody] UpdateCafeStoreRequest request)
    {
        try
        {
            if (id != request.StoreId)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("ID_MISMATCH", "Store ID in URL does not match ID in request body"));
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            var dto = await _mediator.Send(new UpdateCafeStoreCommand(
                request.StoreId,
                request.StoreName,
                request.Address,
                request.PhoneNumber,
                request.Email));

            return Ok(ApiResponse<CafeStoreDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("STORE_NOT_FOUND", $"Cafe store with ID {id} was not found", new { storeId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating cafe store {StoreId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCafeStore(int id)
    {
        try
        {
            await _mediator.Send(new DeleteCafeStoreCommand(id));

            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("STORE_NOT_FOUND", $"Cafe store with ID {id} was not found", new { storeId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting cafe store {StoreId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }
}

public class CreateCafeStoreRequest
{
    [Required]
    [StringLength(255)]
    public string StoreName { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(255)]
    public string? Email { get; set; }
}

public class UpdateCafeStoreRequest : CreateCafeStoreRequest
{
    [Required]
    public int StoreId { get; set; }
}
