
namespace LibraryManagement.Web.Helpers;

/// <summary>
/// Guards controller actions against invalid/abusive paging input from the
/// query string (e.g. pageNumber=-5 or pageSize=100000).
/// </summary>
public static class PaginationHelper
{
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;

    public static int NormalizePageNumber(int pageNumber) => pageNumber < 1 ? 1 : pageNumber;

    public static int NormalizePageSize(int pageSize) =>
        pageSize < 1 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);
}