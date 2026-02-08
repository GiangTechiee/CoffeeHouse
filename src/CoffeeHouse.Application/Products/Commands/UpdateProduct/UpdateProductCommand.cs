using CoffeeHouse.Application.Products.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Products.Commands.UpdateProduct;

/// <summary>
/// Command to update an existing product
/// </summary>
public record UpdateProductCommand : IRequest<ProductDto>
{
    /// <summary>
    /// Product ID to update
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Product name
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Product price
    /// </summary>
    public decimal Price { get; init; }

    /// <summary>
    /// Product description
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Product image URL or path
    /// </summary>
    public string? ImageUrl { get; init; }

    /// <summary>
    /// Additional notes about the product
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// Category ID this product belongs to
    /// </summary>
    public int CategoryId { get; init; }
}
