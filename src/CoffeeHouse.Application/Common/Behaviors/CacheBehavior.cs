using CoffeeHouse.Application.Common.Interfaces;
using CoffeeHouse.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoffeeHouse.Application.Common.Behaviors;

public class CacheBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheable
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CacheBehavior<TRequest, TResponse>> _logger;

    public CacheBehavior(ICacheService cacheService, ILogger<CacheBehavior<TRequest, TResponse>> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking cache for request {RequestName} with key {CacheKey}", typeof(TRequest).Name, request.CacheKey);

        var cachedResponse = await _cacheService.GetAsync<TResponse>(request.CacheKey, cancellationToken);

        if (cachedResponse != null)
        {
            _logger.LogInformation("Cache hit for request {RequestName} with key {CacheKey}", typeof(TRequest).Name, request.CacheKey);
            return cachedResponse;
        }

        _logger.LogInformation("Cache miss for request {RequestName} with key {CacheKey}. Fetching from source.", typeof(TRequest).Name, request.CacheKey);
        
        var response = await next();

        await _cacheService.SetAsync(request.CacheKey, response, request.Expiration, cancellationToken);

        return response;
    }
}
