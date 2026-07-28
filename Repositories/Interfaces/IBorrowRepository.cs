using LibraryManagement.Web.Models.Entities;

namespace LibraryManagement.Web.Repositories.Interfaces;

public interface IBorrowRepository : IGenericRepository<BorrowRecord>
{
    Task<BorrowRecord?> GetByIdWithDetailsAsync(int borrowRecordId);

    /// <summary>The currently-open (not yet returned) borrow record for a given book/member pair, if any.</summary>
    Task<BorrowRecord?> GetActiveBorrowAsync(int bookId, int memberId);

    Task<IEnumerable<BorrowRecord>> GetActiveBorrowsByMemberAsync(int memberId);

    Task<IEnumerable<BorrowRecord>> GetOverdueRecordsAsync();

    /// <summary>Full borrow history, optionally filtered by member and/or book, newest first.</summary>
    Task<IEnumerable<BorrowRecord>> GetHistoryAsync(int? memberId, int? bookId);

    Task<int> CountByStatusAsync(BorrowStatus status);
}
