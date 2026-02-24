using CoffeeHouse.Application.Carts.Commands.AddToCart;
using CoffeeHouse.Application.Carts.Commands.Checkout;
using CoffeeHouse.Application.Carts.Commands.RemoveFromCart;
using CoffeeHouse.Application.Carts.Commands.UpdateCart;
using CoffeeHouse.Application.Carts.DTOs;
using CoffeeHouse.Application.Carts.Queries.GetCart;
using CoffeeHouse.Application.Carts.Queries.GetCartCount;
using CoffeeHouse.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Controllers.API;

/// <summary>
/// RESTful API for Cart management
/// </summary>
[Route("api/v1/cart")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class CartApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CartApiController> _logger;

    public CartApiController(IMediator mediator, ILogger<CartApiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCart([FromQuery] int? customerId = null)
    {
        try
        {
            if (!TryResolveCustomerId(customerId, out var resolvedCustomerId))
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<object>.ErrorResult("FORBIDDEN", "Customer context is missing"));
            }

            var cart = await _mediator.Send(new GetCartQuery { CustomerId = resolvedCustomerId });

            return Ok(ApiResponse<CartDto>.SuccessResult(
                cart,
                new ApiMetadata { RequestId = HttpContext.TraceIdentifier }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving cart");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpGet("count")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCartCount([FromQuery] int? customerId = null)
    {
        try
        {
            if (!TryResolveCustomerId(customerId, out var resolvedCustomerId))
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<object>.ErrorResult("FORBIDDEN", "Customer context is missing"));
            }

            var count = await _mediator.Send(new GetCartCountQuery { CustomerId = resolvedCustomerId });

            return Ok(ApiResponse<int>.SuccessResult(
                count,
                new ApiMetadata { RequestId = HttpContext.TraceIdentifier }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving cart count");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPost("items")]
    [ProducesResponseType(typeof(ApiResponse<AddToCartResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            if (!TryResolveCustomerId(request.CustomerId, out var resolvedCustomerId))
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<object>.ErrorResult("FORBIDDEN", "Customer context is missing"));
            }

            var result = await _mediator.Send(new AddToCartCommand
            {
                CustomerId = resolvedCustomerId,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            });

            return Ok(ApiResponse<AddToCartResult>.SuccessResult(
                result,
                new ApiMetadata { RequestId = HttpContext.TraceIdentifier }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding item to cart");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<UpdateCartResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateCart([FromBody] UpdateCartApiRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            if (!TryResolveCustomerId(request.CustomerId, out var resolvedCustomerId))
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<object>.ErrorResult("FORBIDDEN", "Customer context is missing"));
            }

            var command = new UpdateCartCommand
            {
                CustomerId = resolvedCustomerId,
                Updates = request.Updates
                    .Select(u => new UpdateCartItemRequest
                    {
                        ProductId = u.ProductId,
                        Quantity = u.Quantity
                    }).ToList()
            };

            var result = await _mediator.Send(command);

            return Ok(ApiResponse<UpdateCartResult>.SuccessResult(
                result,
                new ApiMetadata { RequestId = HttpContext.TraceIdentifier }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating cart");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpDelete("items/{productId:int}")]
    [ProducesResponseType(typeof(ApiResponse<RemoveFromCartResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveFromCart(int productId, [FromQuery] int? customerId = null)
    {
        try
        {
            if (!TryResolveCustomerId(customerId, out var resolvedCustomerId))
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<object>.ErrorResult("FORBIDDEN", "Customer context is missing"));
            }

            var result = await _mediator.Send(new RemoveFromCartCommand
            {
                CustomerId = resolvedCustomerId,
                ProductId = productId
            });

            return Ok(ApiResponse<RemoveFromCartResult>.SuccessResult(
                result,
                new ApiMetadata { RequestId = HttpContext.TraceIdentifier }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while removing item from cart");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPost("checkout")]
    [ProducesResponseType(typeof(ApiResponse<CheckoutResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            if (!TryResolveCustomerId(request.CustomerId, out var resolvedCustomerId))
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<object>.ErrorResult("FORBIDDEN", "Customer context is missing"));
            }

            var result = await _mediator.Send(new CheckoutCommand
            {
                CustomerId = resolvedCustomerId,
                CustomerName = request.CustomerName,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                CoffeeShopId = request.CoffeeShopId,
                PaymentMethod = request.PaymentMethod,
                EmployeeId = request.EmployeeId
            });

            return Ok(ApiResponse<CheckoutResult>.SuccessResult(
                result,
                new ApiMetadata { RequestId = HttpContext.TraceIdentifier }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking out cart");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    private bool TryResolveCustomerId(int? requestedCustomerId, out int customerId)
    {
        if (IsStaff() && requestedCustomerId.HasValue)
        {
            customerId = requestedCustomerId.Value;
            return true;
        }

        var claim = User.FindFirst("CustomerId");
        if (claim != null && int.TryParse(claim.Value, out var parsed))
        {
            customerId = parsed;
            return true;
        }

        customerId = 0;
        return false;
    }

    private bool IsStaff()
    {
        return User.IsInRole("Admin") || User.IsInRole("Manager") || User.IsInRole("Employee");
    }
}

public class AddToCartRequest
{
    public int? CustomerId { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Range(1, 999)]
    public int Quantity { get; set; } = 1;
}

public class UpdateCartApiRequest
{
    public int? CustomerId { get; set; }

    [Required]
    [MinLength(1)]
    public List<UpdateCartItemApiRequest> Updates { get; set; } = new();
}

public class UpdateCartItemApiRequest
{
    [Required]
    public int ProductId { get; set; }

    [Range(0, 999)]
    public int Quantity { get; set; }
}

public class CheckoutRequest
{
    public int? CustomerId { get; set; }

    [Required]
    [StringLength(255)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Address { get; set; } = string.Empty;

    [Required]
    public int CoffeeShopId { get; set; }

    [Required]
    [StringLength(50)]
    public string PaymentMethod { get; set; } = string.Empty;

    public int? EmployeeId { get; set; }
}
