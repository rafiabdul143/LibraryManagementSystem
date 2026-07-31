using LibraryManagement.Web.Models.Entities;
using LibraryManagement.Web.Repositories.Interfaces;
using LibraryManagement.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Services.Implementations;

public class MemberService : IMemberService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MemberService> _logger;

    public MemberService(IUnitOfWork unitOfWork, ILogger<MemberService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<Member>> GetAllAsync() => await _unitOfWork.Members.GetAllAsync();

    public async Task<Member?> GetByIdAsync(int memberId) => await _unitOfWork.Members.GetByIdAsync(memberId);

    public async Task<Member?> GetWithHistoryAsync(int memberId) => await _unitOfWork.Members.GetWithBorrowHistoryAsync(memberId);

    public async Task<ServiceResult> CreateAsync(Member member)
    {
        if (await _unitOfWork.Members.GetByEmailAsync(member.Email) is not null)
            return ServiceResult.Failure($"A member with email '{member.Email}' already exists.");

        await _unitOfWork.Members.AddAsync(member);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Member created: {Email}", member.Email);
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> UpdateAsync(Member member)
    {
        var existing = await _unitOfWork.Members.GetByIdAsync(member.MemberId);
        if (existing is null)
            return ServiceResult.Failure("Member not found.");

        var byEmail = await _unitOfWork.Members.GetByEmailAsync(member.Email);
        if (byEmail is not null && byEmail.MemberId != member.MemberId)
            return ServiceResult.Failure($"A member with email '{member.Email}' already exists.");

        existing.FullName = member.FullName;
        existing.Email = member.Email;
        existing.Phone = member.Phone;

        _unitOfWork.Members.Update(existing);
        await _unitOfWork.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteAsync(int memberId)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(memberId);
        if (member is null)
            return ServiceResult.Failure("Member not found.");

        if (await _unitOfWork.Members.HasActiveBorrowsAsync(memberId))
            return ServiceResult.Failure("Cannot delete a member who currently has borrowed books.");

        _unitOfWork.Members.Remove(member);
        await _unitOfWork.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<int> GetTotalCountAsync() => await _unitOfWork.Members.Query().CountAsync();
}
