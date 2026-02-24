using CoffeeHouse.Application.Reports.DTOs;
using MediatR;

namespace CoffeeHouse.Application.Reports.Queries.GetDashboardStats;

public record GetDashboardStatsQuery(string Period = "week") : IRequest<DashboardStatsDto>;
