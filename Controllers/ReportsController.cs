using System.Text;
using LibraryManagement.Web.Services.Interfaces;
using LibraryManagement.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagement.Web.Controllers;

[Authorize(Roles = "Admin")]
public class ReportsController : Controller
{
    private readonly IBorrowService _borrowService;
    private readonly IBookService _bookService;
    private readonly IMemberService _memberService;

    public ReportsController(IBorrowService borrowService, IBookService bookService, IMemberService memberService)
    {
        _borrowService = borrowService;
        _bookService = bookService;
        _memberService = memberService;
    }

    [HttpGet]
    public IActionResult Index() => View();

    // ----------------------------------------------------------------
    // BORROW HISTORY - optional member/book filters
    // ----------------------------------------------------------------
    [HttpGet]
    public async Task<IActionResult> BorrowHistory(int? memberId, int? bookId)
    {
        var model = new BorrowHistoryReportViewModel
        {
            Records = (await _borrowService.GetHistoryAsync(memberId, bookId)).ToList(),
            MemberId = memberId,
            BookId = bookId,
            Members = (await _memberService.GetAllAsync())
                .Select(m => new SelectListItem($"{m.FullName} ({m.Email})", m.MemberId.ToString(), m.MemberId == memberId))
                .ToList(),
            Books = (await _bookService.GetAllAsync())
                .Select(b => new SelectListItem(b.Title, b.BookId.ToString(), b.BookId == bookId))
                .ToList()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ExportBorrowHistoryCsv(int? memberId, int? bookId)
    {
        var records = await _borrowService.GetHistoryAsync(memberId, bookId);

        var csv = new StringBuilder();
        csv.AppendLine("Book,Member,BorrowDate,DueDate,ReturnDate,Status,FineAmount");

        foreach (var r in records)
        {
            csv.AppendLine(string.Join(",",
                CsvEscape(r.Book.Title),
                CsvEscape(r.Member.FullName),
                r.BorrowDate.ToString("yyyy-MM-dd"),
                r.DueDate.ToString("yyyy-MM-dd"),
                r.ReturnDate?.ToString("yyyy-MM-dd") ?? "",
                r.Status.ToString(),
                r.FineAmount.ToString("F2")));
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv", $"borrow-history-{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv");
    }

    // ----------------------------------------------------------------
    // OVERDUE BOOKS
    // ----------------------------------------------------------------
    [HttpGet]
    public async Task<IActionResult> OverdueBooks()
    {
        var records = (await _borrowService.GetOverdueRecordsAsync()).ToList();
        return View(records);
    }

    // ----------------------------------------------------------------
    // AVAILABLE BOOKS - with search
    // ----------------------------------------------------------------
    [HttpGet]
    public async Task<IActionResult> AvailableBooks(string? searchTerm)
    {
        var (items, _) = await _bookService.GetPagedAsync(
            searchTerm, categoryId: null, authorId: null,
            sortColumn: "title", ascending: true, pageNumber: 1, pageSize: 1000);

        var model = new AvailableBooksReportViewModel
        {
            Books = items.Where(b => b.AvailableCopies > 0).ToList(),
            SearchTerm = searchTerm
        };

        return View(model);
    }

    private static string CsvEscape(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return "\"" + value.Replace("\"", "\"\"") + "\"";

        return value;
    }
}