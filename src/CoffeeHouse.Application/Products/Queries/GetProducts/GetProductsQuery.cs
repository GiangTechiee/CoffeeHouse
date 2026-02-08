using CoffeeHouse.Application.Common.Interfaces;
using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Products.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Products.Queries.GetProducts;

/// <summary>
/// Query to get a paginated list of products
/// </summary>
public record GetProductsQuery : IRequest<PagedResult<ProductDto>>, ICacheable
{
    public string CacheKey => $"products-list-{CategoryId}-{SearchTerm}-{MinPrice}-{MaxPrice}-{PageNumber}-{PageSize}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
    /// <summary>
    /// Optional category ID to filter products
    /// </summary>
    public int? CategoryId { get; init; }

    /// <summary>
    /// Optional search term to filter products by name or description
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Optional minimum price filter
    /// </summary>
    public decimal? MinPrice { get; init; }

    /// <summary>
    /// Optional maximum price filter
    /// </summary>
    public decimal? MaxPrice { get; init; }

    /// <summary>
    /// Page number (1-based). Default is 1.
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Number of items per page. Default is 10.
    /// </summary>
    public int PageSize { get; init; } = 10;
}
