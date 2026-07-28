
namespace LibraryManagement.Web.Repositories.Interfaces;

/// <summary>
/// Exposes one instance of each repository, all sharing the same DbContext,
/// plus a single SaveChangesAsync() so multi-repository operations (e.g.
/// issuing a book: update Book.AvailableCopies + insert BorrowRecord)
/// commit atomically in one transaction.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IBookRepository Books { get; }
    IAuthorRepository Authors { get; }
    ICategoryRepository Categories { get; }
    IPublisherRepository Publishers { get; }
    IMemberRepository Members { get; }
    IBorrowRepository BorrowRecords { get; }

    Task<int> SaveChangesAsync();
}