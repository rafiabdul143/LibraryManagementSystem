using LibraryManagement.Web.Models.Entities;

namespace LibraryManagement.Web.Services.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int categoryId);
    Task<ServiceResult> CreateAsync(Category category);
    Task<ServiceResult> UpdateAsync(Category category);
    Task<ServiceResult> DeleteAsync(int categoryId);
}
