using LibraryManagement.Web.Models.Entities;
using LibraryManagement.Web.Repositories.Interfaces;
using LibraryManagement.Web.Services.Interfaces;

namespace LibraryManagement.Web.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(IUnitOfWork unitOfWork, ILogger<CategoryService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<Category>> GetAllAsync() => await _unitOfWork.Categories.GetAllAsync();

    public async Task<Category?> GetByIdAsync(int categoryId) => await _unitOfWork.Categories.GetByIdAsync(categoryId);

    public async Task<ServiceResult> CreateAsync(Category category)
    {
        if (await _unitOfWork.Categories.NameExistsAsync(category.Name))
            return ServiceResult.Failure($"Category '{category.Name}' already exists.");

        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Category created: {Name}", category.Name);
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> UpdateAsync(Category category)
    {
        var existing = await _unitOfWork.Categories.GetByIdAsync(category.CategoryId);
        if (existing is null)
            return ServiceResult.Failure("Category not found.");

        if (await _unitOfWork.Categories.NameExistsAsync(category.Name, category.CategoryId))
            return ServiceResult.Failure($"Category '{category.Name}' already exists.");

        existing.Name = category.Name;
        existing.Description = category.Description;

        _unitOfWork.Categories.Update(existing);
        await _unitOfWork.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteAsync(int categoryId)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
        if (category is null)
            return ServiceResult.Failure("Category not found.");

        if (await _unitOfWork.Categories.HasBooksAsync(categoryId))
            return ServiceResult.Failure("Cannot delete a category that has books assigned to it.");

        _unitOfWork.Categories.Remove(category);
        await _unitOfWork.SaveChangesAsync();
        return ServiceResult.Success();
    }
}
