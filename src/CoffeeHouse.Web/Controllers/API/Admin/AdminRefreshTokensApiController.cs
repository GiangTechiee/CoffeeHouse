using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.RefreshTokens.Commands.DeleteRefreshToken;
using CoffeeHouse.Application.RefreshTokens.Commands.RevokeRefreshToken;
using CoffeeHouse.Application.RefreshTokens.DTOs;
using CoffeeHouse.Application.RefreshTokens.Queries.GetRefreshTokenById;
using CoffeeHouse.Application.RefreshTokens.Queries.GetRefreshTokens;
using CoffeeHouse.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeHouse.Controllers.API.Admin;

/// <summary>
/// RESTful API for refresh token management (Admin)
/// </summary>
[Route("api/v1/admin/refresh-tokens")]
[ApiController]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public class AdminRefreshTokensApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminRefreshTokensApiController> _logger;

    public AdminRefreshTokensApiController(IMediator mediator, ILogger<AdminRefreshTokensApiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<RefreshTokenDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTokens([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] int? userId = null, [FromQuery] bool? activeOnly = null)
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

            var result = await _mediator.Send(new GetRefreshTokensQuery(page, pageSize, userId, activeOnly));
            var response = new PaginatedResponse<RefreshTokenDto>(result.Items.ToList(), result.TotalCount, result.PageNumber, result.PageSize);

            return Ok(ApiResponse<PaginatedResponse<RefreshTokenDto>>.SuccessResult(
                response,
                new ApiMetadata { RequestId = HttpContext.TraceIdentifier }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving refresh tokens");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<RefreshTokenDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetToken(int id)
    {
        try
        {
            var dto = await _mediator.Send(new GetRefreshTokenByIdQuery(id));
            return Ok(ApiResponse<RefreshTokenDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("TOKEN_NOT_FOUND", $"Refresh token with ID {id} was not found", new { tokenId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving refresh token {TokenId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPatch("{id:int}/revoke")]
    [ProducesResponseType(typeof(ApiResponse<RefreshTokenDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeToken(int id)
    {
        try
        {
            var dto = await _mediator.Send(new RevokeRefreshTokenCommand(id));
            return Ok(ApiResponse<RefreshTokenDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("TOKEN_NOT_FOUND", $"Refresh token with ID {id} was not found", new { tokenId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while revoking refresh token {TokenId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteToken(int id)
    {
        try
        {
            await _mediator.Send(new DeleteRefreshTokenCommand(id));
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("TOKEN_NOT_FOUND", $"Refresh token with ID {id} was not found", new { tokenId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting refresh token {TokenId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }
}
