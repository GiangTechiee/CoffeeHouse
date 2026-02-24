using CoffeeHouse.Application.Reports.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Reports.Queries.GetFinanceDetail;

public record GetFinanceDetailQuery(int StoreId, DateTime StartDate, DateTime EndDate) : IRequest<FinanceDetailResponse?>;
