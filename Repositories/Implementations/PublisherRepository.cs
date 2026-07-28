
using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models.Entities;
using LibraryManagement.Web.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Repositories.Implementations;

public class PublisherRepository : GenericRepository<Publisher>, IPublisherRepository
{
    public PublisherRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> HasBooksAsync(int publisherId) =>
        await _context.Books.AnyAsync(b => b.PublisherId == publisherId);
}