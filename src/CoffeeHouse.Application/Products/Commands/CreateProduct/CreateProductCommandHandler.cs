using CoffeeHouse.Application.Products.DTOs;
using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoffeeHouse.Application.Products.Commands.CreateProduct;

/// <summary>
/// Handler for CreateProductCommand
/// </summary>
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of CreateProductCommandHandler
    /// </summary>
    /// <param name="unitOfWork">Unit of work for data access</param>
    /// <param name="logger">Logger instance</param>
    public CreateProductCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Handles the CreateProductCommand
    /// </summary>
    /// <param name="request">The command request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ProductDto representing the created product</returns>
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating product with name: {ProductName}, price: {Price}, categoryId: {CategoryId}",
            request.Name,
            request.Price,
            request.CategoryId);

        // Check if product with same name already exists
        var existingProduct = await _unitOfWork.Products.GetByNameAsync(request.Name, cancellationToken);
        if (existingProduct != null)
        {
            _logger.LogWarning("Product with name {ProductName} already exists", request.Name);
            throw new InvalidOperationException($"Product with name '{request.Name}' already exists");
        }

        // Create domain entity
        var product = new Product
        {
            ProductName = request.Name,
            Price = request.Price,
            CategoryId = request.CategoryId,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            Notes = request.Notes
        };

        // Add product to repository
        await _unitOfWork.Products.AddAsync(product, cancellationToken);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product created successfully with ID: {ProductId}", product.ProductId);

        // Map to DTO and return
        return MapToDto(product);
    }

    /// <summary>
    /// Maps Product entity to ProductDto
    /// </summary>
    /// <param name="product">Product entity</param>
    /// <returns>ProductDto</returns>
    private static ProductDto MapToDto(Product product)
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
