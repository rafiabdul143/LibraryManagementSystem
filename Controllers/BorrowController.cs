using LibraryManagement.Web.Services.Interfaces;
using LibraryManagement.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagement.Web.Controllers;

/// <summary>
/// Librarian operational screen for issuing and returning books. Restricted
/// to Admin - checking books in/out is a staff-desk operation, not a
/// self-service member action in this system's design.
/// </summary>
[Authorize(Roles = "Admin")]
public class BorrowController : Controller
{
	private readonly IBorrowService _borrowService;
	private readonly IBookService _bookService;
	private readonly IMemberService _memberService;
	private readonly ILogger<BorrowController> _logger;

	public BorrowController(
		IBorrowService borrowService,
		IBookService bookService,
		IMemberService memberService,
		ILogger<BorrowController> logger)
	{
		_borrowService = borrowService;
		_bookService = bookService;
		_memberService = memberService;
		_logger = logger;
	}

	// ----------------------------------------------------------------
	// INDEX - all currently active checkouts, overdue highlighted
	// ----------------------------------------------------------------
	[HttpGet]
	public async Task<IActionResult> Index()
	{
		var activeBorrows = await _borrowService.GetAllActiveBorrowsAsync();
		return View(activeBorrows);
	}

	// ----------------------------------------------------------------
	// ISSUE
	// ----------------------------------------------------------------
	[HttpGet]
	public async Task<IActionResult> Issue()
	{
		var model = new IssueBookViewModel();
		await PopulateDropdownsAsync(model);
		return View(model);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Issue(IssueBookViewModel model)
	{
		if (!ModelState.IsValid)
		{
			await PopulateDropdownsAsync(model);
			return View(model);
		}

		var result = await _borrowService.IssueBookAsync(model.BookId, model.MemberId);

		if (!result.Succeeded)
		{
			foreach (var error in result.Errors)
				ModelState.AddModelError(string.Empty, error);

			await PopulateDropdownsAsync(model);
			return View(model);
		}

		_logger.LogInformation("Book {BookId} issued to Member {MemberId} via UI", model.BookId, model.MemberId);
		TempData["SuccessMessage"] = "Book issued successfully.";
		return RedirectToAction(nameof(Index));
	}

	// ----------------------------------------------------------------
	// RETURN
	// ----------------------------------------------------------------
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Return(int borrowRecordId)
	{
		var result = await _borrowService.ReturnBookAsync(borrowRecordId);

		if (!result.Succeeded)
		{
			TempData["ErrorMessage"] = string.Join(" ", result.Errors);
			return RedirectToAction(nameof(Index));
		}

		_logger.LogInformation("BorrowRecord {Id} returned via UI, fine {Fine:C}", borrowRecordId, result.Data);

		TempData["SuccessMessage"] = result.Data > 0
			? $"Book returned successfully. Fine charged: {result.Data:C}."
			: "Book returned successfully. No fine charged.";

		return RedirectToAction(nameof(Index));
	}

	// ----------------------------------------------------------------
	// Private helpers
	// ----------------------------------------------------------------
	private async Task PopulateDropdownsAsync(IssueBookViewModel model)
	{
		var books = await _bookService.GetAllAsync();
		model.Books = books
			.Where(b => b.AvailableCopies > 0)
			.Select(b => new SelectListItem($"{b.Title} ({b.AvailableCopies} available)", b.BookId.ToString()))
			.ToList();

		var members = await _memberService.GetAllAsync();
		model.Members = members
			.Select(m => new SelectListItem($"{m.FullName} ({m.Email})", m.MemberId.ToString()))
			.ToList();
	}
}