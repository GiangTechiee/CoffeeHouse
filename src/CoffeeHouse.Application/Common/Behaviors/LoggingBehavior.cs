using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CoffeeHouse.Application.Common.Behaviors;

/// <summary>
/// Pipeline behavior that logs request execution details including timing
/// </summary>
/// <typeparam name="TRequest">The request type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        
        if (requestName.Contains("Login") || requestName.Contains("Register") || 
            requestName.Contains("Password") || requestName.Contains("UpdateAccountInfo") ||
            requestName.Contains("CreateCustomer"))
        {
             _logger.LogInformation("Handling {RequestName} - Request: [Redacted for Security]", requestName);
        }
        else
        {
            _logger.LogInformation(
                "Handling {RequestName} - Request: {@Request}",
                requestName,
                request);
        }

        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var response = await next();
            
            stopwatch.Stop();
            
            _logger.LogInformation(
                "Handled {RequestName} - Execution time: {ElapsedMilliseconds}ms",
                requestName,
                stopwatch.ElapsedMilliseconds);
            
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            _logger.LogError(
                ex,
                "Error handling {RequestName} - Execution time: {ElapsedMilliseconds}ms",
                requestName,
                stopwatch.ElapsedMilliseconds);
            
            throw;
        }
    }
}
