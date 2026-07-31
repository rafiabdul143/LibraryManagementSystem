using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagement.Web.ViewModels;

/// <summary>
/// Everything the Books/Index view needs: the current page of results,
/// the active search/filter/sort state (so the view can re-render controls
/// with their current values), and dropdown option lists for the filters.
/// </summary>
public class BookIndexViewModel
{
    public PaginatedList<BookListItemViewModel> Books { get; set; } = null!;

    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public int? AuthorId { get; set; }

    public string SortColumn { get; set; } = "title";
    public bool Ascending { get; set; } = true;

    public List<SelectListItem> Categories { get; set; } = new();
    public List<SelectListItem> Authors { get; set; } = new();
}
