
using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models.Entities;
using LibraryManagement.Web.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Repositories.Implementations;

public class MemberRepository : GenericRepository<Member>, IMemberRepository
{
    public MemberRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Member?> GetByEmailAsync(string email) =>
        await _dbSet.FirstOrDefaultAsync(m => m.Email == email);

    public async Task<Member?> GetWithBorrowHistoryAsync(int memberId) =>
        await _dbSet
            .Include(m => m.BorrowRecords)
                .ThenInclude(br => br.Book)
            .FirstOrDefaultAsync(m => m.MemberId == memberId);

    public async Task<bool> HasActiveBorrowsAsync(int memberId) =>
        await _context.BorrowRecords.AnyAsync(br => br.MemberId == memberId && br.Status != BorrowStatus.Returned);
}