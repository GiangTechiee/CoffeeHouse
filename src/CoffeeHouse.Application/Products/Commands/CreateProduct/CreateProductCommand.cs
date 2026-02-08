using CoffeeHouse.Application.Products.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Products.Commands.CreateProduct;

/// <summary>
/// Command to create a new product
/// </summary>
public record CreateProductCommand : IRequest<ProductDto>
{
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
