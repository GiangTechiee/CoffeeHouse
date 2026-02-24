using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoffeeHouse.Infrastructure.Persistence.Repositories;

public class PurchaseOrderRepository : Repository<PurchaseOrder>, IPurchaseOrderRepository
{
    public PurchaseOrderRepository(CoffeeHouseContext context) : base(context)
    {
    }

    public async Task<(IReadOnlyList<PurchaseOrder> Items, int TotalCount)> GetPagedAsync(
        DateTime? searchDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<PurchaseOrder> query = _dbSet.AsNoTracking()
            .Include(p => p.Store)
            .Include(p => p.Supplier)
            .Include(p => p.Employee);

        if (searchDate.HasValue)
        {
            var date = searchDate.Value.Date;
            query = query.Where(p => p.OrderDate.Date == date);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.OrderDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<PurchaseOrder?> GetWithDetailsAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(p => p.Store)
            .Include(p => p.Supplier)
            .Include(p => p.Employee)
            .Include(p => p.PurchaseOrderItems)
                .ThenInclude(i => i.Ingredient)
            .FirstOrDefaultAsync(p => p.PurchaseOrderId == purchaseOrderId, cancellationToken);
    }

    public async Task<PurchaseOrder?> GetForUpdateAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Store)
            .Include(p => p.Supplier)
            .Include(p => p.Employee)
            .Include(p => p.PurchaseOrderItems)
                .ThenInclude(i => i.Ingredient)
            .FirstOrDefaultAsync(p => p.PurchaseOrderId == purchaseOrderId, cancellationToken);
    }

    public Task DeleteWithItemsAsync(PurchaseOrder order, CancellationToken cancellationToken = default)
    {
        var items = _context.Set<PurchaseOrderItem>().Where(i => i.PurchaseOrderId == order.PurchaseOrderId);
        _context.Set<PurchaseOrderItem>().RemoveRange(items);
        _context.Set<PurchaseOrder>().Remove(order);
        return Task.CompletedTask;
    }
}
