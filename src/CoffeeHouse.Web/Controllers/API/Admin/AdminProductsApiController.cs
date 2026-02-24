using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Products.Commands.CreateProduct;
using CoffeeHouse.Application.Products.Commands.DeleteProduct;
using CoffeeHouse.Application.Products.Commands.UpdateProduct;
using CoffeeHouse.Application.Products.DTOs;
using CoffeeHouse.Application.Products.Queries.GetProductById;
using CoffeeHouse.Application.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeHouse.Controllers.API.Admin;

/// <summary>
/// RESTful API for Product management (Admin)
/// </summary>
[Route("api/v1/admin/products")]
[ApiController]
[Authorize(Roles = "Admin,Employee")]
[Produces("application/json")]
public class AdminProductsApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminProductsApiController> _logger;

    public AdminProductsApiController(IMediator mediator, ILogger<AdminProductsApiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<ProductDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? categoryId = null,
        [FromQuery] string? search = null)
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

            var query = new GetProductsQuery
            {
                PageNumber = page,
                PageSize = pageSize,
                CategoryId = categoryId,
                SearchTerm = search
            };

            var result = await _mediator.Send(query);

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving products");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult(
                    "INTERNAL_ERROR",
                    "An error occurred while processing your request"
                )
            );
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct(int id)
    {
        try
        {
            var query = new GetProductByIdQuery { Id = id };
            var product = await _mediator.Send(query);

            return Ok(ApiResponse<ProductDto>.SuccessResult(
                product,
                new ApiMetadata
                {
                    RequestId = HttpContext.TraceIdentifier
                }
            ));
        }
        catch (Domain.Exceptions.NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult(
                "PRODUCT_NOT_FOUND",
                $"Product with ID {id} was not found",
                new { productId = id }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving product {ProductId}", id);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult(
                    "INTERNAL_ERROR",
                    "An error occurred while processing your request"
                )
            );
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
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
                nameof(GetProduct),
                new { id = result.Id },
                ApiResponse<ProductDto>.SuccessResult(
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
            _logger.LogWarning(ex, "Validation error while creating product");
            return UnprocessableEntity(ApiResponse<object>.ErrorResult(
                "VALIDATION_ERROR",
                ex.Message
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating product");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult(
                    "INTERNAL_ERROR",
                    "An error occurred while processing your request"
                )
            );
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(
                    "ID_MISMATCH",
                    "Product ID in URL does not match ID in request body"
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

            return Ok(ApiResponse<ProductDto>.SuccessResult(
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
                "PRODUCT_NOT_FOUND",
                $"Product with ID {id} was not found",
                new { productId = id }
            ));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error while updating product {ProductId}", id);
            return UnprocessableEntity(ApiResponse<object>.ErrorResult(
                "VALIDATION_ERROR",
                ex.Message
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating product {ProductId}", id);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult(
                    "INTERNAL_ERROR",
                    "An error occurred while processing your request"
                )
            );
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            var command = new DeleteProductCommand { Id = id };
            await _mediator.Send(command);

            return NoContent();
        }
        catch (Domain.Exceptions.NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult(
                "PRODUCT_NOT_FOUND",
                $"Product with ID {id} was not found",
                new { productId = id }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting product {ProductId}", id);
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
