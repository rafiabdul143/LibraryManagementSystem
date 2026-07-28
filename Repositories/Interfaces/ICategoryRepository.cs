using LibraryManagement.Web.Models.Entities;

namespace LibraryManagement.Web.Repositories.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<bool> HasBooksAsync(int categoryId);
    Task<bool> NameExistsAsync(string name, int? excludeCategoryId = null);
}
