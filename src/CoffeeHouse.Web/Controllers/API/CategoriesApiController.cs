using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Categories.Commands.CreateCategory;
using CoffeeHouse.Application.Categories.Commands.DeleteCategory;
using CoffeeHouse.Application.Categories.Commands.UpdateCategory;
using CoffeeHouse.Application.Categories.DTOs;
using CoffeeHouse.Application.Categories.Queries.GetCategoryById;
using CoffeeHouse.Application.Categories.Queries.GetCategories;
using CoffeeHouse.Application.Products.DTOs;
using CoffeeHouse.Application.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CoffeeHouse.Controllers.API;

/// <summary>
/// RESTful API for Category management
/// </summary>
[Route("api/v1/categories")]
[ApiController]
[Produces("application/json")]
public class CategoriesApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CategoriesApiController> _logger;

    public CategoriesApiController(IMediator mediator, ILogger<CategoriesApiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all categories with pagination
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 20, max: 100)</param>
    /// <param name="search">Search in category name</param>
    /// <returns>Paginated list of categories</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<CategoryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCategories(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null)
    {
        // Public access
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

            var query = new GetCategoriesQuery
            {
                PageNumber = page,
                PageSize = pageSize,
                SearchTerm = search
            };

            var result = await _mediator.Send(query);

            var paginatedResponse = new PaginatedResponse<CategoryDto>(
                result.Items.ToList(),
                result.TotalCount,
                page,
                pageSize
            );

            return Ok(ApiResponse<PaginatedResponse<CategoryDto>>.SuccessResult(
                paginatedResponse,
                new ApiMetadata
                {
                    RequestId = HttpContext.TraceIdentifier
                }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving categories");
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
    /// Get a specific category by ID
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>Category details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategory(int id)
    {
        // Public access
        try
        {
            var query = new GetCategoryByIdQuery { Id = id };
            var category = await _mediator.Send(query);

            return Ok(ApiResponse<CategoryDto>.SuccessResult(
                category,
                new ApiMetadata
                {
                    RequestId = HttpContext.TraceIdentifier
                }
            ));
        }
        catch (Domain.Exceptions.NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult(
                "CATEGORY_NOT_FOUND",
                $"Category with ID {id} was not found",
                new { categoryId = id }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving category {CategoryId}", id);
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
    /// Get all products in a specific category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Items per page</param>
    /// <returns>Paginated list of products in category</returns>
    [HttpGet("{id}/products")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<ProductDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategoryProducts(
        int id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        // Public access
        try
        {
            // First verify category exists
            var categoryQuery = new GetCategoryByIdQuery { Id = id };
            await _mediator.Send(categoryQuery);

            // Get products in category
            var productsQuery = new GetProductsQuery
            {
                CategoryId = id,
                PageNumber = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(productsQuery);

            var paginatedResponse = new PaginatedResponse<ProductDto>(
                result.Items.ToList(),
                result.TotalCount,
                page,
                pageSize
            );

            return Ok(ApiResponse<PaginatedResponse<ProductDto>>.SuccessResult(
                paginatedResponse,
                new ApiMetadata
                {
                    RequestId = HttpContext.TraceIdentifier
                }
            ));
        }
        catch (Domain.Exceptions.NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult(
                "CATEGORY_NOT_FOUND",
                $"Category with ID {id} was not found",
                new { categoryId = id }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving products for category {CategoryId}", id);
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
    /// Create a new category
    /// </summary>
    /// <param name="command">Category creation data</param>
    /// <returns>Created category</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
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

            var result = await _mediator.Send(command);

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = result.Id },
                ApiResponse<CategoryDto>.SuccessResult(
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
            _logger.LogWarning(ex, "Validation error while creating category");
            return UnprocessableEntity(ApiResponse<object>.ErrorResult(
                "VALIDATION_ERROR",
                ex.Message
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating category");
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
    /// Update an existing category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="command">Updated category data</param>
    /// <returns>Updated category</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(
                    "ID_MISMATCH",
                    "Category ID in URL does not match ID in request body"
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

            return Ok(ApiResponse<CategoryDto>.SuccessResult(
                result,
                new ApiMetadata
                {
                    RequestId = HttpContext.TraceIdentifier
                }
            ));
        }
        catch (Domain.Exceptions.NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult(
                "CATEGORY_NOT_FOUND",
                $"Category with ID {id} was not found",
                new { categoryId = id }
            ));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error while updating category {CategoryId}", id);
            return UnprocessableEntity(ApiResponse<object>.ErrorResult(
                "VALIDATION_ERROR",
                ex.Message
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating category {CategoryId}", id);
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
    /// Delete a category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            var command = new DeleteCategoryCommand(id);
            await _mediator.Send(command);

            return NoContent();
        }
        catch (Domain.Exceptions.NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult(
                "CATEGORY_NOT_FOUND",
                $"Category with ID {id} was not found",
                new { categoryId = id }
            ));
        }
        catch (InvalidOperationException ex)
        {
            // Category has products, cannot delete
            return Conflict(ApiResponse<object>.ErrorResult(
                "CATEGORY_HAS_PRODUCTS",
                ex.Message,
                new { categoryId = id }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting category {CategoryId}", id);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult(
                    "INTERNAL_ERROR",
                    "An error occurred while processing your request"
                )
            );
        }
    }
}
