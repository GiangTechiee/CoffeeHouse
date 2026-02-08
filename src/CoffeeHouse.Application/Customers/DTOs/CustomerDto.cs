namespace CoffeeHouse.Application.Customers.DTOs;

/// <summary>
/// Data Transfer Object for Customer
/// </summary>
public class CustomerDto
{
    /// <summary>
    /// Customer ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Customer full name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Customer phone number
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Customer address
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Date when the customer was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date when the customer was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
