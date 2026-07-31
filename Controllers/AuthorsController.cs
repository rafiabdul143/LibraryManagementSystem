using LibraryManagement.Web.Models.Entities;
using LibraryManagement.Web.Services.Interfaces;
using LibraryManagement.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Web.Controllers;

[Authorize]
public class AuthorsController : Controller
{
    private readonly IAuthorService _authorService;
    private readonly ILogger<AuthorsController> _logger;

    public AuthorsController(IAuthorService authorService, ILogger<AuthorsController> logger)
    {
        _authorService = authorService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var authors = (await _authorService.GetAllAsync()).OrderBy(a => a.FullName).ToList();
        return View(authors);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var author = await _authorService.GetByIdAsync(id);
        if (author is null)
            return NotFound();

        return View(author);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new AuthorViewModel());

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AuthorViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var author = new Author { FullName = model.FullName, Bio = model.Bio };
        var result = await _authorService.CreateAsync(author);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);
            return View(model);
        }

        _logger.LogInformation("Author '{FullName}' created via UI", author.FullName);
        TempData["SuccessMessage"] = $"Author '{author.FullName}' was created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var author = await _authorService.GetByIdAsync(id);
        if (author is null)
            return NotFound();

        var model = new AuthorViewModel
        {
            AuthorId = author.AuthorId,
            FullName = author.FullName,
            Bio = author.Bio
        };

        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AuthorViewModel model)
    {
        if (id != model.AuthorId)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        var author = new Author { AuthorId = model.AuthorId, FullName = model.FullName, Bio = model.Bio };
        var result = await _authorService.UpdateAsync(author);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);
            return View(model);
        }

        _logger.LogInformation("Author {AuthorId} updated via UI", model.AuthorId);
        TempData["SuccessMessage"] = $"Author '{author.FullName}' was updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var author = await _authorService.GetByIdAsync(id);
        if (author is null)
            return NotFound();

        return View(author);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _authorService.DeleteAsync(id);

        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = string.Join(" ", result.Errors);
            return RedirectToAction(nameof(Delete), new { id });
        }

        _logger.LogInformation("Author {AuthorId} deleted via UI", id);
        TempData["SuccessMessage"] = "Author was deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}