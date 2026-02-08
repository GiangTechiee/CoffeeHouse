using CoffeeHouse.Application.Products.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Products.Queries.GetProductById;

/// <summary>
/// Query to get a single product by ID
/// </summary>
public record GetProductByIdQuery : IRequest<ProductDto>
{
    /// <summary>
    /// Product ID to retrieve
    /// </summary>
    public int Id { get; init; }
}
