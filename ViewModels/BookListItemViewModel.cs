
namespace LibraryManagement.Web.ViewModels;

/// <summary>Flattened, read-only projection of a Book for the Index list table.</summary>
public class BookListItemViewModel
{
    public int BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string PublisherName { get; set; } = string.Empty;
    public int PublishedYear { get; set; }
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
}