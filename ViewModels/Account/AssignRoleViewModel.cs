
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Web.ViewModels.Account;

public class AssignRoleViewModel
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    [Required, Display(Name = "Role")]
    public string SelectedRole { get; set; } = string.Empty;

    public List<string> AvailableRoles { get; set; } = new();
}