using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Web.ViewModels;

public class MemberViewModel
{
    public int MemberId { get; set; }

    [Required, StringLength(150)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Phone, StringLength(20)]
    public string? Phone { get; set; }
}