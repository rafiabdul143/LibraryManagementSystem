using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Web.ViewModels;

public class PublisherViewModel
{
    public int PublisherId { get; set; }

    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Address { get; set; }
}