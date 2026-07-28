using LibraryManagement.Web.Models.Entities;

namespace LibraryManagement.Web.Repositories.Interfaces;

public interface IAuthorRepository : IGenericRepository<Author>
{
    /// <summary>Used before delete: prevents removing an author who still has books.</summary>
    Task<bool> HasBooksAsync(int authorId);
}