using CoffeeHouse.Domain.Entities;

namespace CoffeeHouse.Domain.Interfaces;

/// <summary>
/// Repository interface for SalesOrder entity
/// </summary>
public interface IOrderRepository : IRepository<SalesOrder>
{
    Task<SalesOrder?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SalesOrder>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SalesOrder>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SalesOrder>> GetByStoreIdAsync(int storeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SalesOrder>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SalesOrder>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<SalesOrder?> GetWithItemsAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<SalesOrder> Orders, int TotalCount)> GetPagedAsync(
        int? customerId,
        int? employeeId,
        int? storeId,
        string? status,
        DateTime? startDate,
        DateTime? endDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
