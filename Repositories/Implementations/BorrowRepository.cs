using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models.Entities;
using LibraryManagement.Web.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Repositories.Implementations;

public class BorrowRepository : GenericRepository<BorrowRecord>, IBorrowRepository
{
    public BorrowRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<BorrowRecord?> GetByIdWithDetailsAsync(int borrowRecordId) =>
        await _dbSet
            .Include(br => br.Book)
            .Include(br => br.Member)
            .FirstOrDefaultAsync(br => br.BorrowRecordId == borrowRecordId);

    public async Task<BorrowRecord?> GetActiveBorrowAsync(int bookId, int memberId) =>
        await _dbSet.FirstOrDefaultAsync(br =>
            br.BookId == bookId && br.MemberId == memberId && br.Status != BorrowStatus.Returned);

    public async Task<IEnumerable<BorrowRecord>> GetActiveBorrowsByMemberAsync(int memberId) =>
        await _dbSet
            .Include(br => br.Book)
            .Where(br => br.MemberId == memberId && br.Status != BorrowStatus.Returned)
            .ToListAsync();

    public async Task<IEnumerable<BorrowRecord>> GetOverdueRecordsAsync() =>
        await _dbSet
            .Include(br => br.Book)
            .Include(br => br.Member)
            .Where(br => br.Status != BorrowStatus.Returned && br.DueDate < DateTime.UtcNow)
            .OrderBy(br => br.DueDate)
            .ToListAsync();

    public async Task<IEnumerable<BorrowRecord>> GetHistoryAsync(int? memberId, int? bookId)
    {
        var query = _dbSet
            .Include(br => br.Book)
            .Include(br => br.Member)
            .AsQueryable();

        if (memberId.HasValue)
            query = query.Where(br => br.MemberId == memberId.Value);

        if (bookId.HasValue)
            query = query.Where(br => br.BookId == bookId.Value);

        return await query.OrderByDescending(br => br.BorrowDate).ToListAsync();
    }

    public async Task<int> CountByStatusAsync(BorrowStatus status) =>
        await _dbSet.CountAsync(br => br.Status == status);
}
