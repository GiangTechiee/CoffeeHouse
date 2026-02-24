using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.News.Commands.CreateNews;
using CoffeeHouse.Application.News.Commands.DeleteNews;
using CoffeeHouse.Application.News.Commands.UpdateNews;
using CoffeeHouse.Application.News.DTOs;
using CoffeeHouse.Application.News.Queries.GetNews;
using CoffeeHouse.Application.News.Queries.GetNewsById;
using CoffeeHouse.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Controllers.API.Admin;

/// <summary>
/// RESTful API for News management (Admin)
/// </summary>
[Route("api/v1/admin/news")]
[ApiController]
[Authorize(Roles = "Admin,Employee")]
[Produces("application/json")]
public class AdminNewsApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminNewsApiController> _logger;

    public AdminNewsApiController(IMediator mediator, ILogger<AdminNewsApiController> logger)
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

            var result = await _mediator.Send(new GetNewsQuery(page, pageSize, search));
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
            var article = await _mediator.Send(new GetNewsByIdQuery(id));
            return Ok(ApiResponse<NewsArticleDto>.SuccessResult(article, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("NEWS_NOT_FOUND", $"News article with ID {id} was not found", new { articleId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving news article {ArticleId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<NewsArticleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateNews([FromBody] CreateNewsArticleRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            var dto = await _mediator.Send(new CreateNewsCommand(
                request.Title,
                request.Content,
                request.PublishedAt == default ? null : request.PublishedAt,
                request.ImageUrl,
                request.Status));

            return CreatedAtAction(nameof(GetNewsById), new { id = dto.ArticleId },
                ApiResponse<NewsArticleDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating news article");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<NewsArticleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateNews(int id, [FromBody] UpdateNewsArticleRequest request)
    {
        try
        {
            if (id != request.ArticleId)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("ID_MISMATCH", "Article ID in URL does not match ID in request body"));
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ApiResponse<object>.ErrorResult("VALIDATION_ERROR", "Request validation failed", ModelState));
            }

            var dto = await _mediator.Send(new UpdateNewsCommand(
                request.ArticleId,
                request.Title,
                request.Content,
                request.PublishedAt == default ? null : request.PublishedAt,
                request.ImageUrl,
                request.Status));

            return Ok(ApiResponse<NewsArticleDto>.SuccessResult(dto, new ApiMetadata { RequestId = HttpContext.TraceIdentifier }));
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("NEWS_NOT_FOUND", $"News article with ID {id} was not found", new { articleId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating news article {ArticleId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteNews(int id)
    {
        try
        {
            await _mediator.Send(new DeleteNewsCommand(id));

            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResult("NEWS_NOT_FOUND", $"News article with ID {id} was not found", new { articleId = id }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting news article {ArticleId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResult("INTERNAL_ERROR", "An error occurred while processing your request"));
        }
    }
}

public class CreateNewsArticleRequest
{
    [Required]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    [StringLength(20)]
    public string? Status { get; set; }

    public DateTime PublishedAt { get; set; }
}

public class UpdateNewsArticleRequest : CreateNewsArticleRequest
{
    [Required]
    public int ArticleId { get; set; }
}
