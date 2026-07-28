
using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models.Entities;
using LibraryManagement.Web.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Repositories.Implementations;

public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
{
    public AuthorRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> HasBooksAsync(int authorId) =>
        await _context.Books.AnyAsync(b => b.AuthorId == authorId);
}