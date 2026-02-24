using CoffeeHouse.Domain.Entities;

namespace CoffeeHouse.Domain.Interfaces;

public interface INewsRepository : IRepository<NewsArticle>
{
    Task<(IReadOnlyList<NewsArticle> Items, int TotalCount)> GetPagedAsync(
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<NewsArticle> Items, int TotalCount)> GetPagedByStatusAsync(
        string? search,
        string status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
