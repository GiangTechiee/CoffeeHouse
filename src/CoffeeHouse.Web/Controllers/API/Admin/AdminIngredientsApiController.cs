using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Ingredients.Commands.CreateIngredient;
using CoffeeHouse.Application.Ingredients.Commands.DeleteIngredient;
using CoffeeHouse.Application.Ingredients.Commands.UpdateIngredient;
using CoffeeHouse.Application.Ingredients.DTOs;
using CoffeeHouse.Application.Ingredients.Queries.GetIngredientById;
using CoffeeHouse.Application.Ingredients.Queries.GetIngredients;
using CoffeeHouse.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Controllers.API.Admin;

/// <summary>
/// RESTful API for Ingredient management (Admin)
/// </summary>
[Route("api/v1/admin/ingredients")]
[ApiController]
[Authorize(Roles = "Admin,Employee")]
[Produces("application/json")]
public class AdminIngredientsApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminIngredientsApiController> _logger;

    public AdminIngredientsApiController(IMediator mediator, ILogger<AdminIngredientsApiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<IngredientDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetIngredients([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
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

            var result = await _mediator.Send(new GetIngredientsQuery(page, pageSize, search));
            var response = new PaginatedResponse<IngredientDto>(
                result.Items.ToList(),
                result.TotalCount,
                result.PageNumber,
                result.PageSize);

            return Ok(ApiResponse<PaginatedResponse<IngredientDto>>.SuccessResult(
                response,
                new ApiMetadata { RequestId = HttpContext.TraceIdentifier }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving ingredients");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<IngredientDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetIngredient(int id)
    {
        try
        {
            var ingredient = await _mediator.Send(new GetIngredientByIdQuery(id));
            return Ok(ApiResponse<IngredientDto>.SuccessResult(ingredient, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("INGREDIENT_NOT_FOUND", $"Ingredient with ID {id} was not found", new { ingredientId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving ingredient {IngredientId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<IngredientDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateIngredient([FromBody] CreateIngredientRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            var dto = await _mediator.Send(new CreateIngredientCommand(
                request.IngredientName,
                request.Quantity,
                request.Unit,
                request.ExpirationDate,
                request.UnitPrice,
                request.MinimumQuantity));

            return CreatedAtAction(nameof(GetIngredient), new { id = dto.IngredientId },
                ApiResponse<IngredientDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating ingredient");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<IngredientDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateIngredient(int id, [FromBody] UpdateIngredientRequest request)
    {
        try
        {
            if (id != request.IngredientId)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("ID_MISMATCH", "Ingredient ID in URL does not match ID in request body"));
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            var dto = await _mediator.Send(new UpdateIngredientCommand(
                request.IngredientId,
                request.IngredientName,
                request.Quantity,
                request.Unit,
                request.ExpirationDate,
                request.UnitPrice,
                request.MinimumQuantity));

            return Ok(ApiResponse<IngredientDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("INGREDIENT_NOT_FOUND", $"Ingredient with ID {id} was not found", new { ingredientId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating ingredient {IngredientId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteIngredient(int id)
    {
        try
        {
            await _mediator.Send(new DeleteIngredientCommand(id));

            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("INGREDIENT_NOT_FOUND", $"Ingredient with ID {id} was not found", new { ingredientId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting ingredient {IngredientId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }
}

public class CreateIngredientRequest
{
    [Required]
    [StringLength(255)]
    public string IngredientName { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Quantity { get; set; }

    [Required]
    [StringLength(50)]
    public string Unit { get; set; } = string.Empty;

    public DateTime? ExpirationDate { get; set; }

    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    [Range(0, double.MaxValue)]
    public decimal MinimumQuantity { get; set; }
}

public class UpdateIngredientRequest : CreateIngredientRequest
{
    [Required]
    public int IngredientId { get; set; }
}
