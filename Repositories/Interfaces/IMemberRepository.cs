
using LibraryManagement.Web.Models.Entities;

namespace LibraryManagement.Web.Repositories.Interfaces;

public interface IMemberRepository : IGenericRepository<Member>
{
    Task<Member?> GetByEmailAsync(string email);

    /// <summary>Loads a member together with their full borrow history (Book included).</summary>
    Task<Member?> GetWithBorrowHistoryAsync(int memberId);

    /// <summary>Used before delete: prevents removing a member who has books checked out.</summary>
    Task<bool> HasActiveBorrowsAsync(int memberId);
}