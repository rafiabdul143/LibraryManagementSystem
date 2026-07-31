using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Web.ViewModels;

public class CategoryViewModel
{
    public int CategoryId { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}