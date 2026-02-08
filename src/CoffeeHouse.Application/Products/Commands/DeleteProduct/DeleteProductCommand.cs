using MediatR;

namespace CoffeeHouse.Application.Products.Commands.DeleteProduct;

/// <summary>
/// Command to delete a product
/// </summary>
public record DeleteProductCommand : IRequest<bool>
{
    /// <summary>
    /// Product ID to delete
    /// </summary>
    public int Id { get; init; }
}
