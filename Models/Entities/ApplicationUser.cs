using Microsoft.AspNetCore.Identity;

namespace LibraryManagement.Web.Models.Entities;

/// <summary>
/// Extends the default Identity user with application-specific profile data.
/// AspNetUsers table is generated from this class via AddEntityFrameworkStores.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Optional 1:1 link to a Member record - populated when a logged-in
    /// user is also a library member (as opposed to a librarian/Admin-only account).
    /// </summary>
    public int? MemberId { get; set; }
    public Member? Member { get; set; }
}