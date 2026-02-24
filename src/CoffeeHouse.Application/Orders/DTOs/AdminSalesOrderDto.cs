namespace CoffeeHouse.Application.Orders.DTOs;

public class AdminSalesOrderListItemDto
{
    public Guid OrderId { get; set; }
    public int StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public DateTime OrderDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class AdminSalesOrderDetailDto : AdminSalesOrderListItemDto
{
    public List<OrderItemDto> Items { get; set; } = new();
}
