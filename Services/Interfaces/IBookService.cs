
using LibraryManagement.Web.Models.Entities;

namespace LibraryManagement.Web.Services.Interfaces;

public interface IBookService
{
    Task<IEnumerable<Book>> GetAllAsync();
    Task<Book?> GetByIdAsync(int bookId);

    Task<(IEnumerable<Book> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm, int? categoryId, int? authorId,
        string sortColumn, bool ascending, int pageNumber, int pageSize);

    Task<ServiceResult> CreateAsync(Book book);
    Task<ServiceResult> UpdateAsync(Book book);
    Task<ServiceResult> DeleteAsync(int bookId);

    Task<int> GetTotalCountAsync();
    Task<int> GetAvailableCopiesCountAsync();
}