namespace CoffeeHouse.Application.Reports.DTOs;

public class DashboardStatsDto
{
    public decimal TotalRevenue { get; set; }
    public decimal RevenueGrowth { get; set; }
    public int TotalOrders { get; set; }
    public decimal OrderGrowth { get; set; }
    public int TotalCustomers { get; set; }
    public decimal CustomerGrowth { get; set; }
    public decimal ConversionRate { get; set; }
    public decimal ConversionGrowth { get; set; }
}
