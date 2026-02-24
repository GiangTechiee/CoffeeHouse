using CoffeeHouse.Application.Interfaces;
using CoffeeHouse.Application.Reports.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Reports.Queries.GetFinanceDetail;

public class GetFinanceDetailQueryHandler : IRequestHandler<GetFinanceDetailQuery, FinanceDetailResponse?>
{
    private readonly IReportService _reportService;

    public GetFinanceDetailQueryHandler(IReportService reportService)
    {
        _reportService = reportService;
    }

    public Task<FinanceDetailResponse?> Handle(GetFinanceDetailQuery request, CancellationToken cancellationToken)
    {
        return _reportService.GetFinanceDetailAsync(request.StoreId, request.StartDate, request.EndDate, cancellationToken);
    }
}
