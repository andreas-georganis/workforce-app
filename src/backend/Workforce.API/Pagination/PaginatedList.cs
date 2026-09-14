using System.Text.Json;
using System.Text.Json.Serialization;

namespace Workforce.API.Pagination;

public class PaginatedList<T> : List<T>
{
    public PaginatedList(IEnumerable<T> items, int size, int? number = null, int? totalCount = null)
    {
        AddRange(items);

        Size = size;
        Number = number;
        TotalCount = totalCount;

        TotalPages = TotalCount.HasValue ? (int?)Math.Ceiling(TotalCount.Value / (double)Size) : null;

        HasNextPage = Number.HasValue && TotalCount.HasValue ? Number.Value * Size < TotalCount.Value : null;
        HasPreviousPage = Number.HasValue ? Number.Value > 1 : null;
    }

    public int Size { get; }
    public int? Number { get; }

    /// <summary>
    /// The total number of items across all pages, if known.
    /// </summary>
    public int? TotalCount { get; }

    public int? TotalPages { get; }

    public bool? HasNextPage { get; }
    public bool? HasPreviousPage { get; }
}