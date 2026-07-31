using LibraryManagement.Web.Models.Entities;
using LibraryManagement.Web.Repositories.Interfaces;
using LibraryManagement.Web.Services;
using LibraryManagement.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Services.Implementations;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;

    public DashboardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DashboardSummary> GetSummaryAsync()
    {
        var totalBooks = await _unitOfWork.Books.Query().CountAsync();
        var totalMembers = await _unitOfWork.Members.Query().CountAsync();
        var availableBooks = await _unitOfWork.Books.Query().SumAsync(b => b.AvailableCopies);
        var borrowedBooks = await _unitOfWork.BorrowRecords.Query()
            .CountAsync(br => br.Status != BorrowStatus.Returned);
        var overdueBooks = await _unitOfWork.BorrowRecords.Query()
            .CountAsync(br => br.Status != BorrowStatus.Returned && br.DueDate < DateTime.UtcNow);

        return new DashboardSummary(totalBooks, totalMembers, availableBooks, borrowedBooks, overdueBooks);
    }

    public async Task<IEnumerable<CategoryBookCount>> GetCategoryDistributionAsync()
    {
        var result = await _unitOfWork.Books.Query()
            .GroupBy(b => b.Category.Name)
            .Select(g => new
            {
                CategoryName = g.Key,
                BookCount = g.Count()
            })
            .OrderByDescending(x => x.BookCount)
            .ToListAsync();

        return result.Select(x => new CategoryBookCount(
            x.CategoryName,
            x.BookCount));
    }
}