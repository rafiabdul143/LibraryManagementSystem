using LibraryManagement.Web.Models.Entities;
using LibraryManagement.Web.Repositories.Interfaces;
using LibraryManagement.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Services.Implementations;

public class BookService : IBookService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BookService> _logger;

    public BookService(IUnitOfWork unitOfWork, ILogger<BookService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<Book>> GetAllAsync() =>
        await _unitOfWork.Books.GetAllAsync();

    public async Task<Book?> GetByIdAsync(int bookId) =>
        await _unitOfWork.Books.GetByIdWithDetailsAsync(bookId);

    public async Task<(IEnumerable<Book> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm, int? categoryId, int? authorId,
        string sortColumn, bool ascending, int pageNumber, int pageSize) =>
        await _unitOfWork.Books.GetPagedAsync(searchTerm, categoryId, authorId, sortColumn, ascending, pageNumber, pageSize);

    public async Task<ServiceResult> CreateAsync(Book book)
    {
        if (!await _unitOfWork.Books.IsIsbnUniqueAsync(book.ISBN))
            return ServiceResult.Failure($"A book with ISBN '{book.ISBN}' already exists.");

        if (book.AvailableCopies > book.TotalCopies)
            return ServiceResult.Failure("Available copies cannot exceed total copies.");

        await _unitOfWork.Books.AddAsync(book);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Book created: {Title} ({ISBN})", book.Title, book.ISBN);
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> UpdateAsync(Book book)
    {
        var existing = await _unitOfWork.Books.GetByIdAsync(book.BookId);
        if (existing is null)
            return ServiceResult.Failure("Book not found.");

        if (!await _unitOfWork.Books.IsIsbnUniqueAsync(book.ISBN, book.BookId))
            return ServiceResult.Failure($"A book with ISBN '{book.ISBN}' already exists.");

        if (book.AvailableCopies > book.TotalCopies)
            return ServiceResult.Failure("Available copies cannot exceed total copies.");

        existing.Title = book.Title;
        existing.ISBN = book.ISBN;
        existing.PublishedYear = book.PublishedYear;
        existing.TotalCopies = book.TotalCopies;
        existing.AvailableCopies = book.AvailableCopies;
        existing.AuthorId = book.AuthorId;
        existing.CategoryId = book.CategoryId;
        existing.PublisherId = book.PublisherId;

        _unitOfWork.Books.Update(existing);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Book updated: {BookId}", book.BookId);
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteAsync(int bookId)
    {
        var book = await _unitOfWork.Books.GetByIdAsync(bookId);
        if (book is null)
            return ServiceResult.Failure("Book not found.");

        if (book.AvailableCopies < book.TotalCopies)
            return ServiceResult.Failure("Cannot delete a book that currently has copies checked out.");

        _unitOfWork.Books.Remove(book);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Book deleted: {BookId}", bookId);
        return ServiceResult.Success();
    }

    public async Task<int> GetTotalCountAsync() =>
        await _unitOfWork.Books.Query().CountAsync();

    public async Task<int> GetAvailableCopiesCountAsync() =>
        await _unitOfWork.Books.Query().SumAsync(b => b.AvailableCopies);
}
