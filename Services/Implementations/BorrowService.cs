using LibraryManagement.Web.Helpers;
using LibraryManagement.Web.Models;
using LibraryManagement.Web.Models.Entities;
using LibraryManagement.Web.Repositories.Interfaces;
using LibraryManagement.Web.Services;
using LibraryManagement.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LibraryManagement.Web.Services.Implementations;

public class BorrowService : IBorrowService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AppSettings _appSettings;
    private readonly ILogger<BorrowService> _logger;

    public BorrowService(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings, ILogger<BorrowService> logger)
    {
        _unitOfWork = unitOfWork;
        _appSettings = appSettings.Value;
        _logger = logger;
    }

    public async Task<ServiceResult> IssueBookAsync(int bookId, int memberId)
    {
        var book = await _unitOfWork.Books.GetByIdAsync(bookId);
        if (book is null)
            return ServiceResult.Failure("Book not found.");

        var member = await _unitOfWork.Members.GetByIdAsync(memberId);
        if (member is null)
            return ServiceResult.Failure("Member not found.");

        if (book.AvailableCopies <= 0)
            return ServiceResult.Failure($"No available copies of '{book.Title}' to issue.");

        var alreadyBorrowed = await _unitOfWork.BorrowRecords.GetActiveBorrowAsync(bookId, memberId);
        if (alreadyBorrowed is not null)
            return ServiceResult.Failure("This member already has this book checked out.");

        var record = new BorrowRecord
        {
            BookId = bookId,
            MemberId = memberId,
            BorrowDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(_appSettings.BorrowPeriodDays),
            Status = BorrowStatus.Borrowed
        };

        book.AvailableCopies -= 1;

        _unitOfWork.Books.Update(book);
        await _unitOfWork.BorrowRecords.AddAsync(record);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Book {BookId} issued to Member {MemberId}, due {DueDate}", bookId, memberId, record.DueDate);
        return ServiceResult.Success();
    }

    public async Task<ServiceResult<decimal>> ReturnBookAsync(int borrowRecordId)
    {
        var record = await _unitOfWork.BorrowRecords.GetByIdWithDetailsAsync(borrowRecordId);
        if (record is null)
            return ServiceResult<decimal>.Failure("Borrow record not found.");

        if (record.Status == BorrowStatus.Returned)
            return ServiceResult<decimal>.Failure("This book has already been returned.");

        var returnDate = DateTime.UtcNow;
        var fine = FineCalculator.Calculate(record.DueDate, returnDate, _appSettings.FinePerDayAmount);

        record.ReturnDate = returnDate;
        record.FineAmount = fine;
        record.Status = BorrowStatus.Returned;
        record.Book.AvailableCopies += 1;

        _unitOfWork.BorrowRecords.Update(record);
        _unitOfWork.Books.Update(record.Book);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Book returned for BorrowRecord {Id}, fine {Fine:C}", borrowRecordId, fine);
        return ServiceResult<decimal>.Success(fine);
    }

    public async Task<IEnumerable<BorrowRecord>> GetActiveBorrowsByMemberAsync(int memberId) =>
        await _unitOfWork.BorrowRecords.GetActiveBorrowsByMemberAsync(memberId);

    public async Task<IEnumerable<BorrowRecord>> GetAllActiveBorrowsAsync() =>
        await _unitOfWork.BorrowRecords.Query()
            .Include(br => br.Book)
            .Include(br => br.Member)
            .Where(br => br.Status != BorrowStatus.Returned)
            .OrderBy(br => br.DueDate)
            .ToListAsync();

    public async Task<IEnumerable<BorrowRecord>> GetOverdueRecordsAsync() =>
        await _unitOfWork.BorrowRecords.GetOverdueRecordsAsync();

    public async Task<IEnumerable<BorrowRecord>> GetHistoryAsync(int? memberId, int? bookId) =>
        await _unitOfWork.BorrowRecords.GetHistoryAsync(memberId, bookId);

    public async Task<int> GetBorrowedCountAsync() =>
        await _unitOfWork.BorrowRecords.Query().CountAsync(br => br.Status != BorrowStatus.Returned);

    public async Task<int> GetOverdueCountAsync() =>
        await _unitOfWork.BorrowRecords.Query()
            .CountAsync(br => br.Status != BorrowStatus.Returned && br.DueDate < DateTime.UtcNow);

    public async Task UpdateOverdueStatusesAsync()
    {
        var newlyOverdue = await _unitOfWork.BorrowRecords.Query()
            .Where(br => br.Status == BorrowStatus.Borrowed && br.DueDate < DateTime.UtcNow)
            .ToListAsync();

        if (newlyOverdue.Count == 0)
            return;

        foreach (var record in newlyOverdue)
        {
            record.Status = BorrowStatus.Overdue;
            _unitOfWork.BorrowRecords.Update(record);
        }

        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Marked {Count} borrow record(s) as Overdue", newlyOverdue.Count);
    }
}