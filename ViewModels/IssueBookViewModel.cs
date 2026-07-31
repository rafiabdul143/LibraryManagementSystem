using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagement.Web.ViewModels;

/// <summary>Bound to the Borrow/Issue form. Carries dropdown option lists
/// (Books with at least one available copy, all Members) populated by the controller.</summary>
public class IssueBookViewModel
{
    [Required, Display(Name = "Book")]
    public int BookId { get; set; }

    [Required, Display(Name = "Member")]
    public int MemberId { get; set; }

    public List<SelectListItem> Books { get; set; } = new();
    public List<SelectListItem> Members { get; set; } = new();
}