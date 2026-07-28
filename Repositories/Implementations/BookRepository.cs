using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models.Entities;
using LibraryManagement.Web.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Repositories.Implementations;

public class BookRepository : GenericRepository<Book>, IBookRepository
{
    public BookRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Book?> GetByIdWithDetailsAsync(int bookId) =>
        await _dbSet
            .Include(b => b.Author)
            .Include(b => b.Category)
            .Include(b => b.Publisher)
            .FirstOrDefaultAsync(b => b.BookId == bookId);

    public async Task<(IEnumerable<Book> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm,
        int? categoryId,
        int? authorId,
        string sortColumn,
        bool ascending,
        int pageNumber,
        int pageSize)
    {
        var query = _dbSet
            .Include(b => b.Author)
            .Include(b => b.Category)
            .Include(b => b.Publisher)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(b =>
                b.Title.Contains(term) ||
                b.ISBN.Contains(term) ||
                b.Author.FullName.Contains(term));
        }

        if (categoryId.HasValue)
            query = query.Where(b => b.CategoryId == categoryId.Value);

        if (authorId.HasValue)
            query = query.Where(b => b.AuthorId == authorId.Value);

        var totalCount = await query.CountAsync();

        query = sortColumn.ToLowerInvariant() switch
        {
            "title" => ascending ? query.OrderBy(b => b.Title) : query.OrderByDescending(b => b.Title),
            "author" => ascending ? query.OrderBy(b => b.Author.FullName) : query.OrderByDescending(b => b.Author.FullName),
            "year" => ascending ? query.OrderBy(b => b.PublishedYear) : query.OrderByDescending(b => b.PublishedYear),
            "available" => ascending ? query.OrderBy(b => b.AvailableCopies) : query.OrderByDescending(b => b.AvailableCopies),
            _ => ascending ? query.OrderBy(b => b.Title) : query.OrderByDescending(b => b.Title)
        };

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<bool> IsIsbnUniqueAsync(string isbn, int? excludeBookId = null) =>
        !await _dbSet.AnyAsync(b => b.ISBN == isbn && (!excludeBookId.HasValue || b.BookId != excludeBookId.Value));
}
