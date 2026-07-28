
using LibraryManagement.Web.Models.Entities;

namespace LibraryManagement.Web.Repositories.Interfaces;

public interface IPublisherRepository : IGenericRepository<Publisher>
{
    Task<bool> HasBooksAsync(int publisherId);
}