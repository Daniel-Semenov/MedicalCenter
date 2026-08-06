using Profiles.Shared.Common.Constants;

namespace Profiles.Shared.Common.Pagination;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int Page { get; set; } = PaginationConstants.DefaultPage;
    public int PageSize { get; set; } = PaginationConstants.DefaultPageSize;
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPrevious => Page > PaginationConstants.DefaultPage;
    public bool HasNext => Page < TotalPages;
}