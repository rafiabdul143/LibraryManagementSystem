using LibraryManagement.Web.Models.Entities;
using LibraryManagement.Web.Services;

namespace LibraryManagement.Web.Services.Interfaces;

public interface IBorrowService
{
    /// <summary>Issues a book to a member: validates availability, creates the
    /// BorrowRecord with a calculated DueDate, decrements Book.AvailableCopies.</summary>
    Task<ServiceResult> IssueBookAsync(int bookId, int memberId);

    /// <summary>Marks a borrow record returned, calculates any overdue fine,
    /// and increments Book.AvailableCopies. Returns the fine amount charged.</summary>
    Task<ServiceResult<decimal>> ReturnBookAsync(int borrowRecordId);

    Task<IEnumerable<BorrowRecord>> GetActiveBorrowsByMemberAsync(int memberId);

    /// <summary>All currently active (not yet returned) borrow records, across all members,
    /// with Book and Member navigation data loaded - powers the Borrow/Index screen.</summary>
    Task<IEnumerable<BorrowRecord>> GetAllActiveBorrowsAsync();

    Task<IEnumerable<BorrowRecord>> GetOverdueRecordsAsync();
    Task<IEnumerable<BorrowRecord>> GetHistoryAsync(int? memberId, int? bookId);

    Task<int> GetBorrowedCountAsync();
    Task<int> GetOverdueCountAsync();

    /// <summary>Flips any Borrowed record past its DueDate to Overdue status in the DB.</summary>
    Task UpdateOverdueStatusesAsync();
}