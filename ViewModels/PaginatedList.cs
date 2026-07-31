namespace LibraryManagement.Web.ViewModels;

/// <summary>
/// Generic wrapper for any paged result set rendered in a view - carries the
/// current page's items plus enough metadata to render pager controls.
/// </summary>
public class PaginatedList<T>
{
    public List<T> Items { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages { get; }

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PaginatedList(List<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = totalCount == 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}
