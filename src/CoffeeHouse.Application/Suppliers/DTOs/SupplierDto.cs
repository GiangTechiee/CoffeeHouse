namespace CoffeeHouse.Application.Suppliers.DTOs;

public class SupplierDto
{
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Stk { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
