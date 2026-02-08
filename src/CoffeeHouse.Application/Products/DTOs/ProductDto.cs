namespace CoffeeHouse.Application.Products.DTOs;

/// <summary>
/// Data Transfer Object for Product
/// </summary>
public class ProductDto
{
    /// <summary>
    /// Product ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Product name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Product price
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Product description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Product image URL or path
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Additional notes about the product
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Category ID this product belongs to
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Category Name
    /// </summary>
    public string? CategoryName { get; set; }

    /// <summary>
    /// Date when the product was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date when the product was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
