using CoffeeHouse.Application.Common.Exceptions;
using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Users.Commands.CreateUser;
using CoffeeHouse.Application.Users.Commands.DeleteUser;
using CoffeeHouse.Application.Users.Commands.UpdateUser;
using CoffeeHouse.Application.Users.Commands.UpdateUserRoles;
using CoffeeHouse.Application.Users.DTOs;
using CoffeeHouse.Application.Users.Queries.GetUserById;
using CoffeeHouse.Application.Users.Queries.GetUsers;
using CoffeeHouse.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeHouse.Controllers.API.Admin;

/// <summary>
/// RESTful API for User management (Admin)
/// </summary>
[Route("api/v1/admin/users")]
[ApiController]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public class AdminUsersApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminUsersApiController> _logger;

    public AdminUsersApiController(IMediator mediator, ILogger<AdminUsersApiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<UserDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
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

            var result = await _mediator.Send(new GetUsersQuery(page, pageSize, search));
            var response = new PaginatedResponse<UserDto>(
                result.Items.ToList(),
                result.TotalCount,
                result.PageNumber,
                result.PageSize);

            return Ok(ApiResponse<PaginatedResponse<UserDto>>.SuccessResult(
                response,
                new ApiMetadata { RequestId = HttpContext.TraceIdentifier }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving users");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser(int id)
    {
        try
        {
            var dto = await _mediator.Send(new GetUserByIdQuery(id));
            if (dto == null)
            {
                return NotFound(ApiResponse<object>.ErrorResult("USER_NOT_FOUND", "User not found", new { userId = id }));
            }

            return Ok(ApiResponse<UserDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving user {UserId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }
            var dto = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetUser), new { id = dto.Id },
                ApiResponse<UserDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (ApiOperationException ex)
        {
            return ToApiError(ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating user");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("ID_MISMATCH", "User ID in URL does not match ID in request body"));
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            var dto = await _mediator.Send(command);
            return Ok(ApiResponse<UserDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("USER_NOT_FOUND", "User not found", new { userId = id }));
        }
        catch (ApiOperationException ex)
        {
            return ToApiError(ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating user {UserId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPut("{id:int}/roles")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ReplaceRoles(int id, [FromBody] UpdateUserRolesCommand command)
    {
        try
        {
            if (id != command.UserId)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("ID_MISMATCH", "User ID in URL does not match ID in request body"));
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            var dto = await _mediator.Send(command);
            return Ok(ApiResponse<UserDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("USER_NOT_FOUND", "User not found", new { userId = id }));
        }
        catch (ApiOperationException ex)
        {
            return ToApiError(ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating roles for user {UserId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            await _mediator.Send(new DeleteUserCommand(id));

            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("USER_NOT_FOUND", "User not found", new { userId = id }));
        }
        catch (ApiOperationException ex)
        {
            return ToApiError(ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting user {UserId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }
    private IActionResult ToApiError(ApiOperationException ex)
    {
        var status = ex.Code switch
        {
            "ROLE_EXISTS" => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status422UnprocessableEntity
        };

        return StatusCode(status, ApiResponse<object>.ErrorResult(ex.Code, ex.Message, ex.Details));
    }
}
