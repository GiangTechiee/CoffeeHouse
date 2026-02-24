using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Identity;
using CoffeeHouse.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Controllers.API.Admin;

/// <summary>
/// Development-only bootstrap endpoint to seed admin data
/// </summary>
[Route("api/v1/admin/bootstrap")]
[ApiController]
[AllowAnonymous]
[Produces("application/json")]
public class AdminBootstrapApiController : ControllerBase
{
    private readonly CoffeeHouseContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly ILogger<AdminBootstrapApiController> _logger;
    private readonly IHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public AdminBootstrapApiController(
        CoffeeHouseContext context,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        ILogger<AdminBootstrapApiController> logger,
        IHostEnvironment environment,
        IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
        _environment = environment;
        _configuration = configuration;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<BootstrapResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Bootstrap([FromBody] BootstrapRequest request)
    {
        if (!_environment.IsDevelopment())
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiResponse<object>.ErrorResult("FORBIDDEN", "Bootstrap endpoint is only available in Development"));
        }

        var bootstrapToken = _configuration["AdminBootstrap:Token"];
        if (!string.IsNullOrWhiteSpace(bootstrapToken))
        {
            if (!Request.Headers.TryGetValue("X-Bootstrap-Token", out var tokenValue) || tokenValue != bootstrapToken)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<object>.ErrorResult("FORBIDDEN", "Invalid bootstrap token"));
            }
        }

        try
        {
            var roles = request.Roles?.Where(r => !string.IsNullOrWhiteSpace(r)).Select(r => r.Trim()).Distinct().ToList()
                ?? new List<string> { "Admin", "Employee", "Manager", "User" };

            var createdRoles = new List<string>();
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    var createResult = await _roleManager.CreateAsync(new AppRole { Name = role, NormalizedName = role.ToUpperInvariant() });
                    if (createResult.Succeeded)
                    {
                        createdRoles.Add(role);
                    }
                }
            }

            CafeStore? store = null;
            if (request.CafeStore != null)
            {
                store = new CafeStore
                {
                    StoreName = request.CafeStore.StoreName.Trim(),
                    Address = request.CafeStore.Address.Trim(),
                    PhoneNumber = request.CafeStore.PhoneNumber.Trim(),
                    Email = string.IsNullOrWhiteSpace(request.CafeStore.Email) ? null : request.CafeStore.Email.Trim()
                };
                _context.CafeStores.Add(store);
                await _context.SaveChangesAsync();
            }

            AppUser? adminUser = null;
            var adminRequest = request.AdminUser ?? new AdminUserSeed
            {
                UserName = "admin",
                Email = "admin@coffeehouse.com",
                Password = "Admin@12345",
                FullName = "System Admin",
                Roles = new List<string> { "Admin" }
            };

            var adminUserName = string.IsNullOrWhiteSpace(adminRequest.UserName) ? adminRequest.Email : adminRequest.UserName;
            if (!string.IsNullOrWhiteSpace(adminUserName))
            {
                adminUser = await _userManager.FindByNameAsync(adminUserName);
                if (adminUser == null && !string.IsNullOrWhiteSpace(adminRequest.Email))
                {
                    adminUser = await _userManager.FindByEmailAsync(adminRequest.Email);
                }

                if (adminUser == null)
                {
                    adminUser = new AppUser
                    {
                        UserName = adminUserName.Trim(),
                        Email = string.IsNullOrWhiteSpace(adminRequest.Email) ? null : adminRequest.Email.Trim(),
                        FullName = string.IsNullOrWhiteSpace(adminRequest.FullName) ? null : adminRequest.FullName.Trim(),
                        Address = string.IsNullOrWhiteSpace(adminRequest.Address) ? null : adminRequest.Address.Trim()
                    };

                    var createResult = await _userManager.CreateAsync(adminUser, adminRequest.Password);
                    if (!createResult.Succeeded)
                    {
                        return UnprocessableEntity(ApiResponse<object>.ErrorResult("CREATE_USER_FAILED", "Failed to create admin user", createResult.Errors));
                    }
                }

                var adminRoles = adminRequest.Roles?.Where(r => !string.IsNullOrWhiteSpace(r)).Select(r => r.Trim()).Distinct().ToList()
                    ?? new List<string> { "Admin" };

                foreach (var role in adminRoles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(new AppRole { Name = role, NormalizedName = role.ToUpperInvariant() });
                    }
                }

                var currentRoles = await _userManager.GetRolesAsync(adminUser);
                var addRoles = adminRoles.Except(currentRoles).ToList();
                if (addRoles.Count > 0)
                {
                    await _userManager.AddToRolesAsync(adminUser, addRoles);
                }
            }

            var result = new BootstrapResultDto
            {
                RolesCreated = createdRoles,
                StoreId = store?.StoreId,
                StoreName = store?.StoreName,
                AdminUserId = adminUser?.Id,
                AdminUserName = adminUser?.UserName
            };

            return Ok(ApiResponse<BootstrapResultDto>.SuccessResult(result, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while bootstrapping admin data");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }
}

public class BootstrapRequest
{
    public List<string>? Roles { get; set; }
    public CafeStoreSeed? CafeStore { get; set; }
    public AdminUserSeed? AdminUser { get; set; }
}

public class CafeStoreSeed
{
    [Required]
    [StringLength(255)]
    public string StoreName { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(255)]
    public string? Email { get; set; }
}

public class AdminUserSeed
{
    public string? UserName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string Password { get; set; } = string.Empty;

    public string? FullName { get; set; }
    public string? Address { get; set; }
    public List<string>? Roles { get; set; }
}

public class BootstrapResultDto
{
    public List<string> RolesCreated { get; set; } = new();
    public int? StoreId { get; set; }
    public string? StoreName { get; set; }
    public int? AdminUserId { get; set; }
    public string? AdminUserName { get; set; }
}
