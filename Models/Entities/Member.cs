using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Web.Models.Entities;

public class Member
{
    public int MemberId { get; set; }

    [Required, StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Phone, StringLength(20)]
    public string? Phone { get; set; }

    public DateTime MembershipDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Optional link to the Identity user account (nullable string FK to AspNetUsers.Id)
    /// so a member can self-service login and view their own borrow history.
    /// </summary>
    public string? ApplicationUserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }

    public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
}