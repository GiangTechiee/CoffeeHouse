using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Products.Commands.UploadProductImage;
using CoffeeHouse.Application.Products.DTOs;
using CoffeeHouse.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Controllers.API.Admin;

/// <summary>
/// RESTful API for Product image upload (Admin)
/// </summary>
[Route("api/v1/admin/products")]
[ApiController]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public class AdminProductImagesApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminProductImagesApiController> _logger;

    public AdminProductImagesApiController(IMediator mediator, ILogger<AdminProductImagesApiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("{id:int}/image")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<ProductImageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UploadProductImage(int id, [FromForm] ProductImageUploadRequest request)
    {
        try
        {
            if (request.ImageFile == null || request.ImageFile.Length == 0)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Image file is required"));
            }

            if (!IsValidImage(request.ImageFile, out var errorMessage))
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("INVALID_IMAGE", errorMessage));
            }

            var dto = await _mediator.Send(new UploadProductImageCommand(id, request.ImageFile));

            return Ok(ApiResponse<ProductImageDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("PRODUCT_NOT_FOUND", "Product not found", new { productId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while uploading product image for product {ProductId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    private static bool IsValidImage(IFormFile file, out string errorMessage)
    {
        errorMessage = string.Empty;
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            errorMessage = "Only .jpg, .jpeg, .png, .gif files are allowed";
            return false;
        }

        if (file.Length > 5 * 1024 * 1024)
        {
            errorMessage = "Image size must be <= 5MB";
            return false;
        }

        return true;
    }
}

public class ProductImageUploadRequest
{
    [Required]
    public IFormFile ImageFile { get; set; } = null!;
}
