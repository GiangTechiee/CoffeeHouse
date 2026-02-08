using CoffeeHouse.Application.Common.Models;
using CoffeeHouse.Application.Products.DTOs;
using CoffeeHouse.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoffeeHouse.Application.Products.Queries.GetProducts;

/// <summary>
/// Handler for GetProductsQuery
/// </summary>
public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetProductsQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of GetProductsQueryHandler
    /// </summary>
    /// <param name="unitOfWork">Unit of work for data access</param>
    /// <param name="logger">Logger instance</param>
    public GetProductsQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetProductsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetProductsQuery
    /// </summary>
    /// <param name="request">The query request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>PagedResult containing ProductDto list</returns>
    public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting products - CategoryId: {CategoryId}, SearchTerm: {SearchTerm}, MinPrice: {MinPrice}, MaxPrice: {MaxPrice}, Page: {PageNumber}, PageSize: {PageSize}",
            request.CategoryId,
            request.SearchTerm,
            request.MinPrice,
            request.MaxPrice,
            request.PageNumber,
            request.PageSize);

        // Validate pagination parameters
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;
        
        // Limit maximum page size to prevent performance issues
        if (pageSize > 100)
        {
            _logger.LogWarning("Page size {RequestedPageSize} exceeds maximum, limiting to 100", pageSize);
            pageSize = 100;
        }

        // Get paginated products from repository
        var (products, totalCount) = await _unitOfWork.Products.GetPagedAsync(
            request.CategoryId,
            request.SearchTerm,
            request.MinPrice,
            request.MaxPrice,
            pageNumber,
            pageSize,
            cancellationToken);

        _logger.LogInformation(
            "Retrieved {ProductCount} products out of {TotalCount} total",
            products.Count,
            totalCount);

        // Map products to DTOs
        var productDtos = products.Select(MapToDto).ToList();

        // Create paged result
        var pagedResult = new PagedResult<ProductDto>
        {
            Items = productDtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return pagedResult;
    }

    /// <summary>
    /// Maps Product entity to ProductDto
    /// </summary>
    /// <param name="product">Product entity</param>
    /// <returns>ProductDto</returns>
    private static ProductDto MapToDto(Domain.Entities.Product product)
    {
        return new ProductDto
        {
            Id = product.ProductId,
            Name = product.ProductName,
            Price = product.Price,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            Notes = product.Notes,
            CategoryId = product.CategoryId,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
