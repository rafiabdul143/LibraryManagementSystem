namespace LibraryManagement.Web.ViewModels;

/// <summary>Read-only projection of a Book with related entity detail, used by
/// the Details and Delete-confirmation views.</summary>
public class BookDetailsViewModel
{
    public int BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int PublishedYear { get; set; }
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }

    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorBio { get; set; }

    public string CategoryName { get; set; } = string.Empty;
    public string? CategoryDescription { get; set; }

    public string PublisherName { get; set; } = string.Empty;
    public string? PublisherAddress { get; set; }
}
