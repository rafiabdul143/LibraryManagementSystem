
using LibraryManagement.Web.Data;
using LibraryManagement.Web.Repositories.Interfaces;
namespace LibraryManagement.Web.Repositories.Implementations;

/// <summary>
/// Lazily instantiates each repository against one shared ApplicationDbContext
/// so that changes made through multiple repositories within the same request
/// are committed together by a single SaveChangesAsync() call.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    private IBookRepository? _books;
    private IAuthorRepository? _authors;
    private ICategoryRepository? _categories;
    private IPublisherRepository? _publishers;
    private IMemberRepository? _members;
    private IBorrowRepository? _borrowRecords;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IBookRepository Books => _books ??= new BookRepository(_context);
    public IAuthorRepository Authors => _authors ??= new AuthorRepository(_context);
    public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
    public IPublisherRepository Publishers => _publishers ??= new PublisherRepository(_context);
    public IMemberRepository Members => _members ??= new MemberRepository(_context);
    public IBorrowRepository BorrowRecords => _borrowRecords ??= new BorrowRepository(_context);

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}