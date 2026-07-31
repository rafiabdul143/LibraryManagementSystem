using LibraryManagement.Web.Models.Entities;

namespace LibraryManagement.Web.Services.Interfaces;

public interface IPublisherService
{
    Task<IEnumerable<Publisher>> GetAllAsync();
    Task<Publisher?> GetByIdAsync(int publisherId);
    Task<ServiceResult> CreateAsync(Publisher publisher);
    Task<ServiceResult> UpdateAsync(Publisher publisher);
    Task<ServiceResult> DeleteAsync(int publisherId);
}
