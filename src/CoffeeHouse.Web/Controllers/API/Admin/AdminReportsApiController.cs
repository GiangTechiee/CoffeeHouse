using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Reports.DTOs;
using CoffeeHouse.Application.Reports.Queries.GetFinanceDetail;
using CoffeeHouse.Application.Reports.Queries.GetFinanceSummary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeHouse.Controllers.API.Admin;

/// <summary>
/// RESTful API for Finance reports (Admin)
/// </summary>
[Route("api/v1/admin/reports")]
[ApiController]
[Authorize(Roles = "Admin,Employee")]
[Produces("application/json")]
public class AdminReportsApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminReportsApiController> _logger;

    public AdminReportsApiController(IMediator mediator, ILogger<AdminReportsApiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }


    [HttpGet("dashboard-stats")]
    [ProducesResponseType(typeof(ApiResponse<DashboardStatsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardStats([FromQuery] string period = "week")
    {
        try
        {
            var response = await _mediator.Send(new CoffeeHouse.Application.Reports.Queries.GetDashboardStats.GetDashboardStatsQuery(period));

            return Ok(ApiResponse<DashboardStatsDto>.SuccessResult(response, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching dashboard stats");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpGet("finance")]
    [ProducesResponseType(typeof(ApiResponse<FinanceSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFinanceSummary([FromQuery] string? startDate, [FromQuery] string? endDate)
    {
        try
        {
            var range = ResolveDateRange(startDate, endDate);

            var response = await _mediator.Send(new GetFinanceSummaryQuery(range.StartDate, range.EndDate));

            return Ok(ApiResponse<FinanceSummaryResponse>.SuccessResult(response, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while building finance summary");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpGet("finance/{storeId:int}")]
    [ProducesResponseType(typeof(ApiResponse<FinanceDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFinanceDetail(int storeId, [FromQuery] string? startDate, [FromQuery] string? endDate)
    {
        try
        {
            var range = ResolveDateRange(startDate, endDate);

            var response = await _mediator.Send(new GetFinanceDetailQuery(storeId, range.StartDate, range.EndDate));
            if (response == null)
            {
                return NotFound(ApiResponse<object>.ErrorResult("STORE_NOT_FOUND", "Cafe store not found", new { storeId }));
            }

            return Ok(ApiResponse<FinanceDetailResponse>.SuccessResult(response, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while building finance detail for store {StoreId}", storeId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    private static DateRange ResolveDateRange(string? startDate, string? endDate)
    {
        DateTime start;
        DateTime end;

        if (!string.IsNullOrWhiteSpace(startDate) && DateTime.TryParse(startDate, out var startParsed))
        {
            start = startParsed.Date;
        }
        else
        {
            start = DateTime.Today.AddDays(-30);
        }

        if (!string.IsNullOrWhiteSpace(endDate) && DateTime.TryParse(endDate, out var endParsed))
        {
            end = endParsed.Date;
        }
        else
        {
            end = DateTime.Today;
        }

        if (end < start)
        {
            (start, end) = (end, start);
        }

        return new DateRange(start, end);
    }
}

public record DateRange(DateTime StartDate, DateTime EndDate);
