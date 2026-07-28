using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Web.Models.Entities;

public enum BorrowStatus
{
    Borrowed = 0,
    Returned = 1,
    Overdue = 2
}

public class BorrowRecord
{
    public int BorrowRecordId { get; set; }

    public int BookId { get; set; }
    public Book Book { get; set; } = null!;

    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;

    public DateTime BorrowDate { get; set; } = DateTime.UtcNow;

    public DateTime DueDate { get; set; }

    /// <summary>Null while the book is still checked out.</summary>
    public DateTime? ReturnDate { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal FineAmount { get; set; } = 0m;

    public BorrowStatus Status { get; set; } = BorrowStatus.Borrowed;
}