using CoffeeHouse.Application.Interfaces;
using CoffeeHouse.Application.Reports.DTOs;
using CoffeeHouse.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoffeeHouse.Infrastructure.Services;

public class ReportService : IReportService
{
    private readonly CoffeeHouseContext _context;

    public ReportService(CoffeeHouseContext context)
    {
        _context = context;
    }

    public async Task<FinanceSummaryResponse> GetFinanceSummaryAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var start = startDate.Date;
        var end = endDate.Date;

        var stores = await _context.CafeStores.AsNoTracking().ToListAsync(cancellationToken);
        var items = new List<FinanceSummaryItemDto>();

        foreach (var store in stores)
        {
            var totalInvoices = await _context.SalesOrders
                .Where(hd => hd.StoreId == store.StoreId &&
                             hd.OrderDate.Date >= start &&
                             hd.OrderDate.Date <= end)
                .SumAsync(hd => (decimal?)hd.TotalAmount, cancellationToken) ?? 0m;

            var totalSalary = await _context.Employees
                .Where(nv => nv.StoreId == store.StoreId)
                .SumAsync(nv => (decimal?)(nv.BaseSalary * nv.SalaryCoefficient), cancellationToken) ?? 0m;

            var totalImport = await (from ph in _context.PurchaseOrders
                                     join ct in _context.PurchaseOrderItems on ph.PurchaseOrderId equals ct.PurchaseOrderId
                                     where ph.StoreId == store.StoreId &&
                                           ph.OrderDate.Date >= start &&
                                           ph.OrderDate.Date <= end
                                     select (decimal?)ct.LineTotal)
                                     .SumAsync(cancellationToken) ?? 0m;

            var revenue = totalInvoices;
            var cost = totalImport + totalSalary;
            var profit = revenue - cost;

            items.Add(new FinanceSummaryItemDto
            {
                StoreId = store.StoreId,
                StoreName = store.StoreName,
                TotalInvoice = totalInvoices,
                TotalSalary = totalSalary,
                TotalImport = totalImport,
                TotalRevenue = revenue,
                Profit = profit
            });
        }

        return new FinanceSummaryResponse
        {
            StartDate = start,
            EndDate = end,
            Items = items
        };
    }

    public async Task<FinanceDetailResponse?> GetFinanceDetailAsync(int storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var start = startDate.Date;
        var end = endDate.Date;

        var store = await _context.CafeStores.AsNoTracking()
            .FirstOrDefaultAsync(s => s.StoreId == storeId, cancellationToken);

        if (store == null)
        {
            return null;
        }

        var employees = await _context.Employees.AsNoTracking()
            .Where(e => e.StoreId == storeId)
            .Select(e => new EmployeeSalaryDto
            {
                EmployeeId = e.EmployeeId,
                FullName = e.FullName,
                BaseSalary = e.BaseSalary,
                SalaryCoefficient = e.SalaryCoefficient,
                TotalSalary = e.BaseSalary * e.SalaryCoefficient
            })
            .ToListAsync(cancellationToken);

        var invoices = await _context.SalesOrders.AsNoTracking()
            .Where(hd => hd.StoreId == storeId &&
                         hd.OrderDate.Date >= start &&
                         hd.OrderDate.Date <= end)
            .OrderByDescending(hd => hd.OrderDate)
            .Select(hd => new SalesOrderSummaryDto
            {
                OrderId = hd.OrderId,
                OrderDate = hd.OrderDate,
                TotalAmount = hd.TotalAmount,
                Status = hd.Status,
                CustomerId = hd.CustomerId
            })
            .ToListAsync(cancellationToken);

        var purchases = await _context.PurchaseOrders.AsNoTracking()
            .Include(p => p.Supplier)
            .Where(p => p.StoreId == storeId &&
                        p.OrderDate.Date >= start &&
                        p.OrderDate.Date <= end)
            .OrderByDescending(p => p.OrderDate)
            .Select(p => new PurchaseOrderSummaryDto
            {
                PurchaseOrderId = p.PurchaseOrderId,
                OrderDate = p.OrderDate,
                TotalAmount = p.TotalAmount,
                Status = p.Status,
                SupplierId = p.SupplierId,
                SupplierName = p.Supplier.SupplierName
            })
            .ToListAsync(cancellationToken);

        return new FinanceDetailResponse
        {
            StoreId = store.StoreId,
            StoreName = store.StoreName,
            StartDate = start,
            EndDate = end,
            Employees = employees,
            Invoices = invoices,
            Purchases = purchases
        };
    }
}
