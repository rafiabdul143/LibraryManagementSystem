
using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models.Entities;
using LibraryManagement.Web.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Repositories.Implementations;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> HasBooksAsync(int categoryId) =>
        await _context.Books.AnyAsync(b => b.CategoryId == categoryId);

    public async Task<bool> NameExistsAsync(string name, int? excludeCategoryId = null) =>
        await _dbSet.AnyAsync(c => c.Name == name && (!excludeCategoryId.HasValue || c.CategoryId != excludeCategoryId.Value));
}