namespace CoffeeHouse.Application.Orders.DTOs;

/// <summary>
/// Data Transfer Object for Order Item
/// </summary>
public class OrderItemDto
{
    /// <summary>
    /// Product ID
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Product name (snapshot at time of order)
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Quantity ordered
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Unit price at time of order
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Total price for this line item
    /// </summary>
    public decimal TotalPrice { get; set; }
}
