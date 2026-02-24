using CoffeeHouse.Application.Reports.DTOs;

namespace CoffeeHouse.Application.Interfaces;

public interface IReportService
{
    Task<FinanceSummaryResponse> GetFinanceSummaryAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<FinanceDetailResponse?> GetFinanceDetailAsync(int storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
