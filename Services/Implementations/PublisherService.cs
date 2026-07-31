using LibraryManagement.Web.Models.Entities;
using LibraryManagement.Web.Repositories.Interfaces;
using LibraryManagement.Web.Services.Interfaces;

namespace LibraryManagement.Web.Services.Implementations;

public class PublisherService : IPublisherService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PublisherService> _logger;

    public PublisherService(IUnitOfWork unitOfWork, ILogger<PublisherService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<Publisher>> GetAllAsync() => await _unitOfWork.Publishers.GetAllAsync();

    public async Task<Publisher?> GetByIdAsync(int publisherId) => await _unitOfWork.Publishers.GetByIdAsync(publisherId);

    public async Task<ServiceResult> CreateAsync(Publisher publisher)
    {
        await _unitOfWork.Publishers.AddAsync(publisher);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Publisher created: {Name}", publisher.Name);
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> UpdateAsync(Publisher publisher)
    {
        var existing = await _unitOfWork.Publishers.GetByIdAsync(publisher.PublisherId);
        if (existing is null)
            return ServiceResult.Failure("Publisher not found.");

        existing.Name = publisher.Name;
        existing.Address = publisher.Address;

        _unitOfWork.Publishers.Update(existing);
        await _unitOfWork.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteAsync(int publisherId)
    {
        var publisher = await _unitOfWork.Publishers.GetByIdAsync(publisherId);
        if (publisher is null)
            return ServiceResult.Failure("Publisher not found.");

        if (await _unitOfWork.Publishers.HasBooksAsync(publisherId))
            return ServiceResult.Failure("Cannot delete a publisher that has books in the catalog.");

        _unitOfWork.Publishers.Remove(publisher);
        await _unitOfWork.SaveChangesAsync();
        return ServiceResult.Success();
    }
}
