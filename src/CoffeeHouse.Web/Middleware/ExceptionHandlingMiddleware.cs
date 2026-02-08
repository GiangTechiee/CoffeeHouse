using System.Net;
using System.Text.Json;
using CoffeeHouse.Domain.Exceptions;

namespace CoffeeHouse.Middleware;

/// <summary>
/// Middleware for handling exceptions globally and returning consistent error responses
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse();

        switch (exception)
        {
            case ValidationException validationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.StatusCode = response.StatusCode;
                errorResponse.Message = validationException.Message;
                errorResponse.Errors = validationException.Errors;
                
                _logger.LogWarning(validationException, 
                    "Validation error occurred: {Message}", 
                    validationException.Message);
                break;

            case NotFoundException notFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.StatusCode = response.StatusCode;
                errorResponse.Message = notFoundException.Message;
                
                _logger.LogWarning(notFoundException, 
                    "Resource not found: {Message}", 
                    notFoundException.Message);
                break;

            case UnauthorizedException unauthorizedException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.StatusCode = response.StatusCode;
                errorResponse.Message = unauthorizedException.Message;
                
                _logger.LogWarning(unauthorizedException, 
                    "Unauthorized access attempt: {Message}", 
                    unauthorizedException.Message);
                break;

            case DomainException domainException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.StatusCode = response.StatusCode;
                errorResponse.Message = domainException.Message;
                
                _logger.LogWarning(domainException, 
                    "Domain error occurred: {Message}", 
                    domainException.Message);
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.StatusCode = response.StatusCode;
                errorResponse.Message = "An internal server error occurred. Please try again later.";
                
                // Log full exception details for internal server errors
                _logger.LogError(exception, 
                    "Unhandled exception occurred: {Message}", 
                    exception.Message);
                break;
        }

        // Check if request is API or explicitly asks for JSON
        if (context.Request.Path.StartsWithSegments("/api") || 
            context.Request.Headers.Accept.ToString().Contains("application/json"))
        {
            var result = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await response.WriteAsync(result);
        }
        else
        {
            // For MVC requests, redirect to error page
            if (!context.Response.HasStarted)
            {
                context.Response.Redirect("/Home/Error");
            }
        }
    }
}

/// <summary>
/// Standard error response model
/// </summary>
public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public IDictionary<string, string[]>? Errors { get; set; }
}
