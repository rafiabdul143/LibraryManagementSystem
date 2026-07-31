using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Web.ViewModels;

public class AuthorViewModel
{
    public int AuthorId { get; set; }

    [Required, StringLength(150)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Bio { get; set; }
}