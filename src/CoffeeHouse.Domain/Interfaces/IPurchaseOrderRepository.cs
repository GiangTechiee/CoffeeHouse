using CoffeeHouse.Domain.Entities;

namespace CoffeeHouse.Domain.Interfaces;

public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
{
    Task<(IReadOnlyList<PurchaseOrder> Items, int TotalCount)> GetPagedAsync(
        DateTime? searchDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<PurchaseOrder?> GetWithDetailsAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default);

    Task<PurchaseOrder?> GetForUpdateAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default);

    Task DeleteWithItemsAsync(PurchaseOrder order, CancellationToken cancellationToken = default);
}
