namespace CoffeeHouse.Application.Orders.DTOs;

/// <summary>
/// Data Transfer Object for Order
/// </summary>
public class OrderDto
{
    /// <summary>
    /// Order database ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Unique order identifier (GUID)
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// Coffee shop ID where order was placed
    /// </summary>
    public int CoffeeShopId { get; set; }

    /// <summary>
    /// Date when order was created
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// Employee ID who processed the order (nullable)
    /// </summary>
    public int? EmployeeId { get; set; }

    /// <summary>
    /// Customer ID who placed the order
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Payment method
    /// </summary>
    public string PaymentMethod { get; set; } = string.Empty;

    /// <summary>
    /// Total amount for the order
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Order status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Collection of order items
    /// </summary>
    public List<OrderItemDto> Items { get; set; } = new();

    /// <summary>
    /// Date when the order was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date when the order was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
