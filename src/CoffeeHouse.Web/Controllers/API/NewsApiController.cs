using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.News.DTOs;
using CoffeeHouse.Application.News.Queries.GetPublishedNews;
using CoffeeHouse.Application.News.Queries.GetNewsById;
using CoffeeHouse.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeHouse.Controllers.API;

/// <summary>
/// RESTful API for public news
/// </summary>
[Route("api/v1/news")]
[ApiController]
[Produces("application/json")]
public class NewsApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<NewsApiController> _logger;

    public NewsApiController(IMediator mediator, ILogger<NewsApiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<NewsArticleDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNews([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
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

            var result = await _mediator.Send(new GetPublishedNewsQuery(page, pageSize, search));
            var response = new PaginatedResponse<NewsArticleDto>(
                result.Items.ToList(),
                result.TotalCount,
                result.PageNumber,
                result.PageSize);

            return Ok(ApiResponse<PaginatedResponse<NewsArticleDto>>.SuccessResult(
                response,
                new ApiMetadata { RequestId = HttpContext.TraceIdentifier }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving news");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<NewsArticleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetNewsById(int id)
    {
        try
        {
            var dto = await _mediator.Send(new GetNewsByIdQuery(id));
            if (!string.Equals(dto.Status, "Published", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(ApiResponse<object>.ErrorResult("NEWS_NOT_FOUND", "News article not found", new { articleId = id }));
            }

            return Ok(ApiResponse<NewsArticleDto>.SuccessResult(
                dto,
                new ApiMetadata { RequestId = HttpContext.TraceIdentifier }
            ));
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("NEWS_NOT_FOUND", "News article not found", new { articleId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving news article {ArticleId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }
}
