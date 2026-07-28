using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Web.Models.Entities;

public class Book
{
    public int BookId { get; set; }

    [Required, StringLength(250)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string ISBN { get; set; } = string.Empty;

    [Range(1000, 9999)]
    public int PublishedYear { get; set; }

    [Range(0, int.MaxValue)]
    public int TotalCopies { get; set; }

    [Range(0, int.MaxValue)]
    public int AvailableCopies { get; set; }

    // Foreign keys
    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int PublisherId { get; set; }
    public Publisher Publisher { get; set; } = null!;

    // Navigation: one book has many borrow records over its lifetime
    public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
}