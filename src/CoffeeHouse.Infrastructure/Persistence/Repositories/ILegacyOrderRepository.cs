using CoffeeHouse.Domain.Entities;

namespace CoffeeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository interface for legacy order operations during migration
/// </summary>
public interface ILegacyOrderRepository
{
    /// <summary>
    /// Adds a legacy order (SalesOrder)
    /// </summary>
    Task AddOrderAsync(SalesOrder order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a legacy order detail (SalesOrderItem)
    /// </summary>
    Task AddOrderDetailAsync(SalesOrderItem orderDetail, CancellationToken cancellationToken = default);
}
