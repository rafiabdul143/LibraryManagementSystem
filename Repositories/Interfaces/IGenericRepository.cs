
using System.Linq.Expressions;

namespace LibraryManagement.Web.Repositories.Interfaces;

/// <summary>
/// Generic CRUD contract shared by every entity-specific repository.
/// </summary>
public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);

    /// <summary>
    /// Exposes a raw IQueryable for specific repositories to compose
    /// Include()/Where()/OrderBy() chains without duplicating query logic here.
    /// </summary>
    IQueryable<T> Query();
}