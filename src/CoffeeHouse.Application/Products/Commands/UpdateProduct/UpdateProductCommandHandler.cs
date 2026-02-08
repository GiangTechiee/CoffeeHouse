using CoffeeHouse.Application.Products.DTOs;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoffeeHouse.Application.Products.Commands.UpdateProduct;

/// <summary>
/// Handler for UpdateProductCommand
/// </summary>
public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of UpdateProductCommandHandler
    /// </summary>
    /// <param name="unitOfWork">Unit of work for data access</param>
    /// <param name="logger">Logger instance</param>
    public UpdateProductCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Handles the UpdateProductCommand
    /// </summary>
    /// <param name="request">The command request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ProductDto representing the updated product</returns>
    /// <exception cref="NotFoundException">Thrown when product is not found</exception>
    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating product with ID: {ProductId}, name: {ProductName}, price: {Price}, categoryId: {CategoryId}",
            request.Id,
            request.Name,
            request.Price,
            request.CategoryId);

        // Get existing product
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found", request.Id);
            throw new NotFoundException($"Product with ID {request.Id} not found");
        }

        // Check if another product with the same name exists (excluding current product)
        var existingProduct = await _unitOfWork.Products.GetByNameAsync(request.Name, cancellationToken);
        if (existingProduct != null && existingProduct.ProductId != request.Id)
        {
            _logger.LogWarning(
                "Product with name {ProductName} already exists with ID {ExistingProductId}",
                request.Name,
                existingProduct.ProductId);
            throw new InvalidOperationException($"Product with name '{request.Name}' already exists");
        }

        // Update domain entity
        product.ProductName = request.Name;
        product.Description = request.Description;
        product.ImageUrl = request.ImageUrl;
        product.Notes = request.Notes;
        product.Price = request.Price;
        product.CategoryId = request.CategoryId;

        // Update product in repository
        _unitOfWork.Products.Update(product);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product with ID {ProductId} updated successfully", product.ProductId);

        // Map to DTO and return
        return MapToDto(product);
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
