using CoffeeHouse.Application.Common.Exceptions;
using CoffeeHouse.Application.Reports.DTOs;
using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Interfaces;
using MediatR;

namespace CoffeeHouse.Application.Reports.Queries.GetDashboardStats;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;

    public GetDashboardStatsQueryHandler(IOrderRepository orderRepository, ICustomerRepository customerRepository)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
    }

    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        // Define periods (default to week)
        DateTime endDate = DateTime.UtcNow;
        // Fix StartDate calculation to respect the requested period properly
        // If "week", start date is 7 days ago.
        DateTime startDate = request.Period.ToLower() == "month" 
            ? endDate.AddMonths(-1) 
            : endDate.AddDays(-7);
            
        // Previous period is the same duration before startDate
        DateTime prevEndDate = startDate;
        DateTime prevStartDate = request.Period.ToLower() == "month" 
            ? prevEndDate.AddMonths(-1) 
            : prevEndDate.AddDays(-7);

        // Fetch current period data
        // Note: GetByDateRangeAsync likely returns orders where OrderDate is between start and end using database logic
        var currentOrders = await _orderRepository.GetByDateRangeAsync(startDate, endDate, cancellationToken);
        var validCurrentOrders = currentOrders.Where(o => o.Status != "Cancelled").ToList();
        
        // Fetch previous period data
        var prevOrders = await _orderRepository.GetByDateRangeAsync(prevStartDate, prevEndDate, cancellationToken);
        var validPrevOrders = prevOrders.Where(o => o.Status != "Cancelled").ToList();

        // Calculate Revenue
        decimal currentRevenue = validCurrentOrders.Sum(o => o.TotalAmount);
        decimal prevRevenue = validPrevOrders.Sum(o => o.TotalAmount);
        
        // Calculate Orders count
        int currentOrderCount = validCurrentOrders.Count;
        int prevOrderCount = validPrevOrders.Count;

        // Calculate Customers count
        // We use GetAllAsync because there isn't a GetByDateRange for customers in the interface shown
        // This might be inefficient for large datasets but is acceptable for now.
        // A better approach would be to add CountAsync to repository with specification, but we are fixing UI first.
        var allCustomers = await _customerRepository.GetAllAsync(cancellationToken);
        int totalCustomers = allCustomers.Count;
        
        // Calculate NEW customers in periods for growth calculation
        // Using CreatedAt from BaseEntity
        int currentNewCustomers = allCustomers.Count(c => c.CreatedAt >= startDate && c.CreatedAt <= endDate);
        int prevNewCustomers = allCustomers.Count(c => c.CreatedAt >= prevStartDate && c.CreatedAt <= prevEndDate);
        
        // Use total customers for the Value, but new customers for Growth? 
        // The dashboard usually shows "Total Active Customers" and growth in "Active Customers" or "New Customers".
        // Let's use Total Customers for value, and New Customer Growth for growth.
        // Or if we want Total Customer Growth, we compare Total at EndDate vs Total at StartDate.
        // Total at EndDate = allCustomers.Count(c => c.CreatedAt <= endDate);
        // Total at StartDate = allCustomers.Count(c => c.CreatedAt <= startDate);
        int totalAtEnd = allCustomers.Count(c => c.CreatedAt <= endDate);
        int totalAtPrevEnd = allCustomers.Count(c => c.CreatedAt <= startDate);
        
        decimal customerGrowth = CalculateGrowth(totalAtEnd, totalAtPrevEnd);

        // Calculate Growth percentages for others
        decimal revenueGrowth = CalculateGrowth(currentRevenue, prevRevenue);
        decimal orderGrowth = CalculateGrowth(currentOrderCount, prevOrderCount);

        return new DashboardStatsDto
        {
            TotalRevenue = currentRevenue,
            RevenueGrowth = revenueGrowth,
            TotalOrders = currentOrderCount,
            OrderGrowth = orderGrowth,
            TotalCustomers = totalCustomers,
            CustomerGrowth = customerGrowth,
            ConversionRate = 0, // Placeholder as we don't track visits
            ConversionGrowth = 0 // Placeholder
        };
    }

    private decimal CalculateGrowth(decimal current, decimal previous)
    {
        if (previous == 0) return current > 0 ? 100 : 0;
        return Math.Round(((current - previous) / previous) * 100, 1);
    }
}
