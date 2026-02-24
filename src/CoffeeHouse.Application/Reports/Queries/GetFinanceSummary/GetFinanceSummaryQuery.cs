using CoffeeHouse.Application.Reports.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Reports.Queries.GetFinanceSummary;

public record GetFinanceSummaryQuery(DateTime StartDate, DateTime EndDate) : IRequest<FinanceSummaryResponse>;
