namespace Workforce.API.Pagination;

public static class PagingExtensions
{
    extension<T>(IQueryable<T> queryable)
    {
        public async Task<PaginatedList<T>> ToOffsetPagedListAsync(int pageNumber, int pageSize)
        {
            return await OffsetPaginator.PageAsync(queryable, pageNumber, pageSize);
        }

        public async Task<PaginatedList<T>> ToKeysetPagedListAsync(int pageNumber)
        {
            //return await KeysetPaginator.PageAsync(queryable, pageNumber, pageSize, maxPageSize);
            return default;
        }
    }



}
