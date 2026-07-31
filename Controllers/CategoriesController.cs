using LibraryManagement.Web.Models.Entities;
using LibraryManagement.Web.Services.Interfaces;
using LibraryManagement.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Web.Controllers;

[Authorize]
public class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var categories = (await _categoryService.GetAllAsync()).OrderBy(c => c.Name).ToList();
        return View(categories);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category is null)
            return NotFound();

        return View(category);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new CategoryViewModel());

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var category = new Category { Name = model.Name, Description = model.Description };
        var result = await _categoryService.CreateAsync(category);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);
            return View(model);
        }

        _logger.LogInformation("Category '{Name}' created via UI", category.Name);
        TempData["SuccessMessage"] = $"Category '{category.Name}' was created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category is null)
            return NotFound();

        var model = new CategoryViewModel
        {
            CategoryId = category.CategoryId,
            Name = category.Name,
            Description = category.Description
        };

        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryViewModel model)
    {
        if (id != model.CategoryId)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        var category = new Category { CategoryId = model.CategoryId, Name = model.Name, Description = model.Description };
        var result = await _categoryService.UpdateAsync(category);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);
            return View(model);
        }

        _logger.LogInformation("Category {CategoryId} updated via UI", model.CategoryId);
        TempData["SuccessMessage"] = $"Category '{category.Name}' was updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category is null)
            return NotFound();

        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _categoryService.DeleteAsync(id);

        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = string.Join(" ", result.Errors);
            return RedirectToAction(nameof(Delete), new { id });
        }

        _logger.LogInformation("Category {CategoryId} deleted via UI", id);
        TempData["SuccessMessage"] = "Category was deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}