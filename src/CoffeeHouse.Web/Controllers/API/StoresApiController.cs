using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.CafeStores.Queries.GetCafeStores;
using CoffeeHouse.Application.CafeStores.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeHouse.Controllers.API;

[Route("api/v1/stores")]
[ApiController]
[Produces("application/json")]
public class StoresApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<StoresApiController> _logger;

    public StoresApiController(IMediator mediator, ILogger<StoresApiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetStores(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100)
    {
        try
        {
            var query = new GetCafeStoresQuery(page, pageSize);
            var result = await _mediator.Send(query);

            return Ok(ApiResponse<PagedResult<CafeStoreDto>>.SuccessResult(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving stores");
            return StatusCode(500, ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred"));
        }
    }
}
