using LibraryManagement.Web.Models.Entities;

namespace LibraryManagement.Web.Services.Interfaces;

public interface IMemberService
{
    Task<IEnumerable<Member>> GetAllAsync();
    Task<Member?> GetByIdAsync(int memberId);
    Task<Member?> GetWithHistoryAsync(int memberId);
    Task<ServiceResult> CreateAsync(Member member);
    Task<ServiceResult> UpdateAsync(Member member);
    Task<ServiceResult> DeleteAsync(int memberId);
    Task<int> GetTotalCountAsync();
}
