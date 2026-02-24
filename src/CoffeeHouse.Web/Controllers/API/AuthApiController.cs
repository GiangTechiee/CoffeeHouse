using CoffeeHouse.Application.Authentication.Commands.Login;
using CoffeeHouse.Application.Authentication.Commands.RegisterCustomer;
using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace CoffeeHouse.Controllers.API;

/// <summary>
/// RESTful API for Authentication
/// </summary>
[Route("api/v1/auth")]
[ApiController]
[Produces("application/json")]
[EnableRateLimiting("AuthPolicy")]
public class AuthApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthApiController> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public AuthApiController(IMediator mediator, ILogger<AuthApiController> logger, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        _mediator = mediator;
        _logger = logger;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
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

            if (!result.Success)
            {
                return Unauthorized(ApiResponse<object>.ErrorResult(
                    "INVALID_CREDENTIALS",
                    result.Message ?? "Invalid username or password"
                ));
            }

            var response = new LoginResponse
            {
                Id = result.Id,
                Email = result.Email ?? string.Empty,
                Name = result.FullName ?? result.Email,
                Role = result.Role ?? "Customer",
                Token = result.Token ?? string.Empty
            };

            return Ok(ApiResponse<LoginResponse>.SuccessResult(
                response,
                new ApiMetadata
                {
                    RequestId = HttpContext.TraceIdentifier
                }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during login");
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
    /// Register a new user account
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<RegisterResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Register([FromBody] RegisterCustomerCommand command)
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

            if (!result.Success)
            {
                if (result.Message?.Contains("exists") == true)
                {
                    return Conflict(ApiResponse<object>.ErrorResult(
                        "USER_ALREADY_EXISTS",
                        result.Message
                    ));
                }
                
                return BadRequest(ApiResponse<object>.ErrorResult(
                    "REGISTRATION_FAILED",
                    result.Message ?? "Registration failed"
                ));
            }

            var response = new RegisterResponse
            {
                Id = result.CustomerId ?? result.EmployeeId ?? 0,
                Email = command.Email,
                Name = result.Email ?? command.Email,
                Message = result.Message ?? "Registration successful"
            };

            return CreatedAtAction(
                nameof(Login),
                null,
                ApiResponse<RegisterResponse>.SuccessResult(
                    response,
                    new ApiMetadata
                    {
                        RequestId = HttpContext.TraceIdentifier
                    }
                )
            );
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult(
                "VALIDATION_ERROR",
                "Validation failed",
                ex.Errors.GroupBy(e => e.PropertyName)
                         .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during registration");
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
    /// Logout current user
    /// </summary>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Logout()
    {
        // For JWT, logout is client-side (delete token).
        // We can't invalidate the token server-side without a blacklist/db.
        // Just return 204. By default, we might want to clear cookies if used.
        return NoContent();
    }

    /// <summary>
    /// Get current authenticated user information
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserInfoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        try
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub) ?? User.FindFirst(ClaimTypes.NameIdentifier);
            var userNameClaim = User.FindFirst(JwtRegisteredClaimNames.UniqueName) ?? User.FindFirst(ClaimTypes.Name);
            var emailClaim = User.FindFirst(JwtRegisteredClaimNames.Email) ?? User.FindFirst(ClaimTypes.Email);
            var roleClaim = User.FindFirst(ClaimTypes.Role) ?? User.FindFirst("role");

            if (userIdClaim == null)
            {
                 return Unauthorized(ApiResponse<object>.ErrorResult(
                    "NOT_AUTHENTICATED",
                    "User is not authenticated"
                ));
            }

            int.TryParse(userIdClaim.Value, out int userId);

            var userInfo = new UserInfoResponse
            {
                Id = userId,
                Name = userNameClaim?.Value ?? string.Empty,
                Email = emailClaim?.Value ?? string.Empty,
                Role = roleClaim?.Value ?? "Customer" // Default to Customer if role not found
            };

            return Ok(ApiResponse<UserInfoResponse>.SuccessResult(
                userInfo,
                new ApiMetadata
                {
                    RequestId = HttpContext.TraceIdentifier
                }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting current user");
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
    /// Seed default users and roles for development (Admin, Employee)
    /// </summary>
    [HttpPost("seed")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SeedUsers()
    {
        try
        {
            var roles = new[] { "Admin", "Employee", "Customer" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new AppRole { Name = role, NormalizedName = role.ToUpper(), Description = $"{role} Role" });
                }
            }

            // Seed Admin
            var adminEmail = "admin@coffeehouse.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };
                var result = await _userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
            else
            {
                if (!await _userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Seed Employee
            var employeeEmail = "staff@coffeehouse.com";
            var employeeUser = await _userManager.FindByEmailAsync(employeeEmail);
            if (employeeUser == null)
            {
                employeeUser = new AppUser
                {
                    UserName = "staff",
                    Email = employeeEmail,
                    FullName = "Staff Member",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };
                var result = await _userManager.CreateAsync(employeeUser, "Staff123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(employeeUser, "Employee");
                }
            }

            return Ok(new { message = "Users seeded successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding users");
            return StatusCode(500, new { message = "Seeding failed", error = ex.Message });
        }
    }
}

// Response DTOs
public class LoginResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}

public class RegisterResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Message { get; set; } = "Registration successful";
}

public class UserInfoResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
