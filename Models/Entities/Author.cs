using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Web.Models.Entities;

public class Author
{
    public int AuthorId { get; set; }

    [Required, StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Bio { get; set; }

    // Navigation: one author writes many books
    public ICollection<Book> Books { get; set; } = new List<Book>();
}