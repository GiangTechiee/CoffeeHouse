using CoffeeHouse.Application.Common.Exceptions;
using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Roles.Commands.CreateRole;
using CoffeeHouse.Application.Roles.Commands.DeleteRole;
using CoffeeHouse.Application.Roles.Commands.UpdateRole;
using CoffeeHouse.Application.Roles.DTOs;
using CoffeeHouse.Application.Roles.Queries.GetRoles;
using CoffeeHouse.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Controllers.API.Admin;

/// <summary>
/// RESTful API for Role management (Admin)
/// </summary>
[Route("api/v1/admin/roles")]
[ApiController]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public class AdminRolesApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminRolesApiController> _logger;

    public AdminRolesApiController(IMediator mediator, ILogger<AdminRolesApiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoles()
    {
        try
        {
            var roles = await _mediator.Send(new GetRolesQuery());

            return Ok(ApiResponse<List<RoleDto>>.SuccessResult(roles, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving roles");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            var dto = await _mediator.Send(new CreateRoleCommand(request.Name, request.Description));

            return CreatedAtAction(nameof(GetRoles), null,
                ApiResponse<RoleDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (ApiOperationException ex)
        {
            return ToApiError(ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating role");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleRequest request)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("ID_MISMATCH", "Role ID in URL does not match ID in request body"));
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            var dto = await _mediator.Send(new UpdateRoleCommand(request.Id, request.Name, request.Description));

            return Ok(ApiResponse<RoleDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("ROLE_NOT_FOUND", "Role not found", new { roleId = id }));
        }
        catch (ApiOperationException ex)
        {
            return ToApiError(ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating role {RoleId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRole(int id)
    {
        try
        {
            await _mediator.Send(new DeleteRoleCommand(id));

            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("ROLE_NOT_FOUND", "Role not found", new { roleId = id }));
        }
        catch (ApiOperationException ex)
        {
            return ToApiError(ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting role {RoleId}", id);
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

public class CreateRoleRequest
{
    [Required]
    [StringLength(256)]
    public string Name { get; set; } = string.Empty;

    [StringLength(512)]
    public string? Description { get; set; }
}

public class UpdateRoleRequest : CreateRoleRequest
{
    [Required]
    public int Id { get; set; }
}
