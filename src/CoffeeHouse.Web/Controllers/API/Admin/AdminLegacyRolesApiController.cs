using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.LegacyRoles.Commands.CreateLegacyRole;
using CoffeeHouse.Application.LegacyRoles.Commands.DeleteLegacyRole;
using CoffeeHouse.Application.LegacyRoles.Commands.UpdateLegacyRole;
using CoffeeHouse.Application.LegacyRoles.DTOs;
using CoffeeHouse.Application.LegacyRoles.Queries.GetLegacyRoleById;
using CoffeeHouse.Application.LegacyRoles.Queries.GetLegacyRoles;
using CoffeeHouse.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Controllers.API.Admin;

/// <summary>
/// RESTful API for legacy role management (Admin)
/// </summary>
[Route("api/v1/admin/legacy-roles")]
[ApiController]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public class AdminLegacyRolesApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminLegacyRolesApiController> _logger;

    public AdminLegacyRolesApiController(IMediator mediator, ILogger<AdminLegacyRolesApiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<LegacyRoleDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoles()
    {
        try
        {
            var roles = await _mediator.Send(new GetLegacyRolesQuery());
            return Ok(ApiResponse<List<LegacyRoleDto>>.SuccessResult(
                roles,
                new ApiMetadata { RequestId = HttpContext.TraceIdentifier }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving legacy roles");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<LegacyRoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRole(int id)
    {
        try
        {
            var role = await _mediator.Send(new GetLegacyRoleByIdQuery(id));
            return Ok(ApiResponse<LegacyRoleDto>.SuccessResult(
                role,
                new ApiMetadata { RequestId = HttpContext.TraceIdentifier }
            ));
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("ROLE_NOT_FOUND", $"Role with ID {id} was not found", new { roleId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving legacy role {RoleId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<LegacyRoleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateRole([FromBody] LegacyRoleRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            var role = await _mediator.Send(new CreateLegacyRoleCommand(request.Name, request.Description));
            return CreatedAtAction(nameof(GetRole), new { id = role.RoleId },
                ApiResponse<LegacyRoleDto>.SuccessResult(role, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(ApiResponse<object>.ErrorResult("ROLE_EXISTS", ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating legacy role");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<LegacyRoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] LegacyRoleRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            var role = await _mediator.Send(new UpdateLegacyRoleCommand(id, request.Name, request.Description));
            return Ok(ApiResponse<LegacyRoleDto>.SuccessResult(role, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(ApiResponse<object>.ErrorResult("ROLE_EXISTS", ex.Message));
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("ROLE_NOT_FOUND", $"Role with ID {id} was not found", new { roleId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating legacy role {RoleId}", id);
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
            await _mediator.Send(new DeleteLegacyRoleCommand(id));
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("ROLE_NOT_FOUND", $"Role with ID {id} was not found", new { roleId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting legacy role {RoleId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }
}

public class LegacyRoleRequest
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}
