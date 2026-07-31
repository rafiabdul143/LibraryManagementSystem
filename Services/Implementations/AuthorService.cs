using LibraryManagement.Web.Models.Entities;
using LibraryManagement.Web.Repositories.Interfaces;
using LibraryManagement.Web.Services.Interfaces;

namespace LibraryManagement.Web.Services.Implementations;

public class AuthorService : IAuthorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AuthorService> _logger;

    public AuthorService(IUnitOfWork unitOfWork, ILogger<AuthorService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<Author>> GetAllAsync() => await _unitOfWork.Authors.GetAllAsync();

    public async Task<Author?> GetByIdAsync(int authorId) => await _unitOfWork.Authors.GetByIdAsync(authorId);

    public async Task<ServiceResult> CreateAsync(Author author)
    {
        await _unitOfWork.Authors.AddAsync(author);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Author created: {FullName}", author.FullName);
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> UpdateAsync(Author author)
    {
        var existing = await _unitOfWork.Authors.GetByIdAsync(author.AuthorId);
        if (existing is null)
            return ServiceResult.Failure("Author not found.");

        existing.FullName = author.FullName;
        existing.Bio = author.Bio;

        _unitOfWork.Authors.Update(existing);
        await _unitOfWork.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteAsync(int authorId)
    {
        var author = await _unitOfWork.Authors.GetByIdAsync(authorId);
        if (author is null)
            return ServiceResult.Failure("Author not found.");

        if (await _unitOfWork.Authors.HasBooksAsync(authorId))
            return ServiceResult.Failure("Cannot delete an author who has books in the catalog.");

        _unitOfWork.Authors.Remove(author);
        await _unitOfWork.SaveChangesAsync();
        return ServiceResult.Success();
    }
}
