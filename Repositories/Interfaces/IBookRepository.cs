using LibraryManagement.Web.Models.Entities;

namespace LibraryManagement.Web.Repositories.Interfaces;

public interface IBookRepository : IGenericRepository<Book>
{
    /// <summary>Loads a book with Author/Category/Publisher navigation properties populated.</summary>
    Task<Book?> GetByIdWithDetailsAsync(int bookId);

    /// <summary>
    /// Server-side search, filter, sort and page over the book catalog.
    /// Returns the page of results plus the total matching count (for pager UI).
    /// </summary>
    Task<(IEnumerable<Book> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm,
        int? categoryId,
        int? authorId,
        string sortColumn,
        bool ascending,
        int pageNumber,
        int pageSize);

    Task<bool> IsIsbnUniqueAsync(string isbn, int? excludeBookId = null);
}
