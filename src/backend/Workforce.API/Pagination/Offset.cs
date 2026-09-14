using Microsoft.EntityFrameworkCore;

namespace Workforce.API.Pagination;

public static class OffsetPaginator
{
    public static async Task<PaginatedList<T>> PageAsync<T>(
        IQueryable<T> query,
        int page = 1,
        int pageSize = 25,
        int maxPageSize = 100,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pageSize, maxPageSize);
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);

        var skip = checked((page - 1) * pageSize);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, pageSize, page, totalCount);
    }
}
