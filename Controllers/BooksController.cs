using LibraryManagement.Web.Helpers;
using LibraryManagement.Web.Models.Entities;
using LibraryManagement.Web.Services.Interfaces;
using LibraryManagement.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagement.Web.Controllers;

[Authorize]
public class BooksController : Controller
{
    private readonly IBookService _bookService;
    private readonly IAuthorService _authorService;
    private readonly ICategoryService _categoryService;
    private readonly IPublisherService _publisherService;
    private readonly ILogger<BooksController> _logger;

    public BooksController(
        IBookService bookService,
        IAuthorService authorService,
        ICategoryService categoryService,
        IPublisherService publisherService,
        ILogger<BooksController> logger)
    {
        _bookService = bookService;
        _authorService = authorService;
        _categoryService = categoryService;
        _publisherService = publisherService;
        _logger = logger;
    }

    // ----------------------------------------------------------------
    // INDEX - search, filter, sort, page
    // ----------------------------------------------------------------
    [HttpGet]
    public async Task<IActionResult> Index(
        string? searchTerm,
        int? categoryId,
        int? authorId,
        string sortColumn = "title",
        bool ascending = true,
        int pageNumber = 1,
        int pageSize = PaginationHelper.DefaultPageSize)
    {
        pageNumber = PaginationHelper.NormalizePageNumber(pageNumber);
        pageSize = PaginationHelper.NormalizePageSize(pageSize);

        var (items, totalCount) = await _bookService.GetPagedAsync(
            searchTerm, categoryId, authorId, sortColumn, ascending, pageNumber, pageSize);

        var listItems = items.Select(b => new BookListItemViewModel
        {
            BookId = b.BookId,
            Title = b.Title,
            ISBN = b.ISBN,
            AuthorName = b.Author.FullName,
            CategoryName = b.Category.Name,
            PublisherName = b.Publisher.Name,
            PublishedYear = b.PublishedYear,
            TotalCopies = b.TotalCopies,
            AvailableCopies = b.AvailableCopies
        }).ToList();

        var model = new BookIndexViewModel
        {
            Books = new PaginatedList<BookListItemViewModel>(listItems, totalCount, pageNumber, pageSize),
            SearchTerm = searchTerm,
            CategoryId = categoryId,
            AuthorId = authorId,
            SortColumn = sortColumn,
            Ascending = ascending,
            Categories = await BuildCategoryOptionsAsync(categoryId),
            Authors = await BuildAuthorOptionsAsync(authorId)
        };

        return View(model);
    }

    // ----------------------------------------------------------------
    // DETAILS
    // ----------------------------------------------------------------
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var book = await _bookService.GetByIdAsync(id);
        if (book is null)
            return NotFound();

        return View(MapToDetailsViewModel(book));
    }

    // ----------------------------------------------------------------
    // CREATE (Admin only)
    // ----------------------------------------------------------------
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        var model = new BookViewModel();
        await PopulateDropdownsAsync(model);
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(model);
            return View(model);
        }

        var book = new Book
        {
            Title = model.Title,
            ISBN = model.ISBN,
            PublishedYear = model.PublishedYear,
            TotalCopies = model.TotalCopies,
            AvailableCopies = model.AvailableCopies,
            AuthorId = model.AuthorId,
            CategoryId = model.CategoryId,
            PublisherId = model.PublisherId
        };

        var result = await _bookService.CreateAsync(book);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);

            await PopulateDropdownsAsync(model);
            return View(model);
        }

        _logger.LogInformation("Book '{Title}' created via UI", book.Title);
        TempData["SuccessMessage"] = $"Book '{book.Title}' was created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ----------------------------------------------------------------
    // EDIT (Admin only)
    // ----------------------------------------------------------------
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _bookService.GetByIdAsync(id);
        if (book is null)
            return NotFound();

        var model = new BookViewModel
        {
            BookId = book.BookId,
            Title = book.Title,
            ISBN = book.ISBN,
            PublishedYear = book.PublishedYear,
            TotalCopies = book.TotalCopies,
            AvailableCopies = book.AvailableCopies,
            AuthorId = book.AuthorId,
            CategoryId = book.CategoryId,
            PublisherId = book.PublisherId
        };

        await PopulateDropdownsAsync(model);
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BookViewModel model)
    {
        if (id != model.BookId)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(model);
            return View(model);
        }

        var book = new Book
        {
            BookId = model.BookId,
            Title = model.Title,
            ISBN = model.ISBN,
            PublishedYear = model.PublishedYear,
            TotalCopies = model.TotalCopies,
            AvailableCopies = model.AvailableCopies,
            AuthorId = model.AuthorId,
            CategoryId = model.CategoryId,
            PublisherId = model.PublisherId
        };

        var result = await _bookService.UpdateAsync(book);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);

            await PopulateDropdownsAsync(model);
            return View(model);
        }

        _logger.LogInformation("Book {BookId} updated via UI", model.BookId);
        TempData["SuccessMessage"] = $"Book '{book.Title}' was updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ----------------------------------------------------------------
    // DELETE (Admin only)
    // ----------------------------------------------------------------
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _bookService.GetByIdAsync(id);
        if (book is null)
            return NotFound();

        return View(MapToDetailsViewModel(book));
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _bookService.DeleteAsync(id);

        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = string.Join(" ", result.Errors);
            return RedirectToAction(nameof(Delete), new { id });
        }

        _logger.LogInformation("Book {BookId} deleted via UI", id);
        TempData["SuccessMessage"] = "Book was deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ----------------------------------------------------------------
    // Private helpers
    // ----------------------------------------------------------------
    private static BookDetailsViewModel MapToDetailsViewModel(Book book) => new()
    {
        BookId = book.BookId,
        Title = book.Title,
        ISBN = book.ISBN,
        PublishedYear = book.PublishedYear,
        TotalCopies = book.TotalCopies,
        AvailableCopies = book.AvailableCopies,
        AuthorName = book.Author.FullName,
        AuthorBio = book.Author.Bio,
        CategoryName = book.Category.Name,
        CategoryDescription = book.Category.Description,
        PublisherName = book.Publisher.Name,
        PublisherAddress = book.Publisher.Address
    };

    private async Task PopulateDropdownsAsync(BookViewModel model)
    {
        model.Authors = (await _authorService.GetAllAsync())
            .Select(a => new SelectListItem(a.FullName, a.AuthorId.ToString()))
            .ToList();

        model.Categories = (await _categoryService.GetAllAsync())
            .Select(c => new SelectListItem(c.Name, c.CategoryId.ToString()))
            .ToList();

        model.Publishers = (await _publisherService.GetAllAsync())
            .Select(p => new SelectListItem(p.Name, p.PublisherId.ToString()))
            .ToList();
    }

    private async Task<List<SelectListItem>> BuildCategoryOptionsAsync(int? selectedId)
    {
        var categories = await _categoryService.GetAllAsync();
        return categories
            .Select(c => new SelectListItem(c.Name, c.CategoryId.ToString(), c.CategoryId == selectedId))
            .ToList();
    }

    private async Task<List<SelectListItem>> BuildAuthorOptionsAsync(int? selectedId)
    {
        var authors = await _authorService.GetAllAsync();
        return authors
            .Select(a => new SelectListItem(a.FullName, a.AuthorId.ToString(), a.AuthorId == selectedId))
            .ToList();
    }
}
