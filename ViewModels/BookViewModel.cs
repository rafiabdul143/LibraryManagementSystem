
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagement.Web.ViewModels;

/// <summary>
/// Bound to the Create and Edit forms for a Book. Carries dropdown option
/// lists (Authors/Categories/Publishers) so the view has no direct
/// dependency on services - the controller populates these.
/// </summary>
public class BookViewModel
{
    public int BookId { get; set; }

    [Required, StringLength(250)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(20)]
    [Display(Name = "ISBN")]
    public string ISBN { get; set; } = string.Empty;

    [Required]
    [Range(1000, 9999, ErrorMessage = "Enter a valid 4-digit year.")]
    [Display(Name = "Published Year")]
    public int PublishedYear { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Total copies cannot be negative.")]
    [Display(Name = "Total Copies")]
    public int TotalCopies { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Available copies cannot be negative.")]
    [Display(Name = "Available Copies")]
    public int AvailableCopies { get; set; }

    [Required, Display(Name = "Author")]
    public int AuthorId { get; set; }

    [Required, Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Required, Display(Name = "Publisher")]
    public int PublisherId { get; set; }

    public List<SelectListItem> Authors { get; set; } = new();
    public List<SelectListItem> Categories { get; set; } = new();
    public List<SelectListItem> Publishers { get; set; } = new();
}