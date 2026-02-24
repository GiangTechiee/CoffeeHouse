using CoffeeHouse.Application.Authentication.DTOs;
using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.LegacyAccounts.Commands.CreateLegacyAccount;
using CoffeeHouse.Application.LegacyAccounts.Commands.DeleteLegacyAccount;
using CoffeeHouse.Application.LegacyAccounts.Commands.UpdateLegacyAccount;
using CoffeeHouse.Application.LegacyAccounts.Queries.GetLegacyAccountById;
using CoffeeHouse.Application.LegacyAccounts.Queries.GetLegacyAccounts;
using CoffeeHouse.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Controllers.API.Admin;

/// <summary>
/// RESTful API for legacy account management (Admin)
/// </summary>
[Route("api/v1/admin/legacy-accounts")]
[ApiController]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public class AdminLegacyAccountsApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminLegacyAccountsApiController> _logger;

    public AdminLegacyAccountsApiController(IMediator mediator, ILogger<AdminLegacyAccountsApiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<AccountInfoDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAccounts([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
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

            var result = await _mediator.Send(new GetLegacyAccountsQuery(page, pageSize, search));
            var response = new PaginatedResponse<AccountInfoDto>(result.Items.ToList(), result.TotalCount, result.PageNumber, result.PageSize);

            return Ok(ApiResponse<PaginatedResponse<AccountInfoDto>>.SuccessResult(
                response,
                new ApiMetadata { RequestId = HttpContext.TraceIdentifier }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving legacy accounts");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<AccountInfoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccount(int id)
    {
        try
        {
            var dto = await _mediator.Send(new GetLegacyAccountByIdQuery(id));
            return Ok(ApiResponse<AccountInfoDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("ACCOUNT_NOT_FOUND", $"Account with ID {id} was not found", new { accountId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving legacy account {AccountId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AccountInfoDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateAccount([FromBody] CreateLegacyAccountRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            var dto = await _mediator.Send(new CreateLegacyAccountCommand(
                request.Username,
                request.Password,
                request.RoleId,
                request.Status,
                request.EmployeeId,
                request.CustomerId));

            return CreatedAtAction(nameof(GetAccount), new { id = dto.AccountId },
                ApiResponse<AccountInfoDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", ex.Message));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResult("NOT_FOUND", ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating legacy account");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<AccountInfoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateAccount(int id, [FromBody] UpdateLegacyAccountRequest request)
    {
        try
        {
            if (id != request.AccountId)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("ID_MISMATCH", "Account ID in URL does not match ID in request body"));
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            var dto = await _mediator.Send(new UpdateLegacyAccountCommand(
                request.AccountId,
                request.Username,
                request.Password,
                request.RoleId,
                request.Status,
                request.EmployeeId,
                request.CustomerId));

            return Ok(ApiResponse<AccountInfoDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", ex.Message));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResult("NOT_FOUND", ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating legacy account {AccountId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAccount(int id)
    {
        try
        {
            await _mediator.Send(new DeleteLegacyAccountCommand(id));
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("ACCOUNT_NOT_FOUND", $"Account with ID {id} was not found", new { accountId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting legacy account {AccountId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }
}

public class CreateLegacyAccountRequest
{
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public int RoleId { get; set; }

    [StringLength(20)]
    public string? Status { get; set; }

    public int? EmployeeId { get; set; }
    public int? CustomerId { get; set; }
}

public class UpdateLegacyAccountRequest
{
    [Required]
    public int AccountId { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Password { get; set; }

    [Required]
    public int RoleId { get; set; }

    [StringLength(20)]
    public string? Status { get; set; }

    public int? EmployeeId { get; set; }
    public int? CustomerId { get; set; }
}
