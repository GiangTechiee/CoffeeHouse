using CoffeeHouse.Application.Interfaces;
using CoffeeHouse.Application.Reports.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Reports.Queries.GetFinanceSummary;

public class GetFinanceSummaryQueryHandler : IRequestHandler<GetFinanceSummaryQuery, FinanceSummaryResponse>
{
    private readonly IReportService _reportService;

    public GetFinanceSummaryQueryHandler(IReportService reportService)
    {
        _reportService = reportService;
    }

    public Task<FinanceSummaryResponse> Handle(GetFinanceSummaryQuery request, CancellationToken cancellationToken)
    {
        return _reportService.GetFinanceSummaryAsync(request.StartDate, request.EndDate, cancellationToken);
    }
}
