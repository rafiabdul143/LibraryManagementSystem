
using LibraryManagement.Web.Models.Entities;

namespace LibraryManagement.Web.Services.Interfaces;

public interface IAuthorService
{
    Task<IEnumerable<Author>> GetAllAsync();
    Task<Author?> GetByIdAsync(int authorId);
    Task<ServiceResult> CreateAsync(Author author);
    Task<ServiceResult> UpdateAsync(Author author);
    Task<ServiceResult> DeleteAsync(int authorId);
}