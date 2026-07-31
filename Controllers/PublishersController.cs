using LibraryManagement.Web.Models.Entities;
using LibraryManagement.Web.Services.Interfaces;
using LibraryManagement.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Web.Controllers;

[Authorize]
public class PublishersController : Controller
{
    private readonly IPublisherService _publisherService;
    private readonly ILogger<PublishersController> _logger;

    public PublishersController(IPublisherService publisherService, ILogger<PublishersController> logger)
    {
        _publisherService = publisherService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var publishers = (await _publisherService.GetAllAsync()).OrderBy(p => p.Name).ToList();
        return View(publishers);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var publisher = await _publisherService.GetByIdAsync(id);
        if (publisher is null)
            return NotFound();

        return View(publisher);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new PublisherViewModel());

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PublisherViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var publisher = new Publisher { Name = model.Name, Address = model.Address };
        var result = await _publisherService.CreateAsync(publisher);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);
            return View(model);
        }

        _logger.LogInformation("Publisher '{Name}' created via UI", publisher.Name);
        TempData["SuccessMessage"] = $"Publisher '{publisher.Name}' was created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var publisher = await _publisherService.GetByIdAsync(id);
        if (publisher is null)
            return NotFound();

        var model = new PublisherViewModel
        {
            PublisherId = publisher.PublisherId,
            Name = publisher.Name,
            Address = publisher.Address
        };

        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PublisherViewModel model)
    {
        if (id != model.PublisherId)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        var publisher = new Publisher { PublisherId = model.PublisherId, Name = model.Name, Address = model.Address };
        var result = await _publisherService.UpdateAsync(publisher);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);
            return View(model);
        }

        _logger.LogInformation("Publisher {PublisherId} updated via UI", model.PublisherId);
        TempData["SuccessMessage"] = $"Publisher '{publisher.Name}' was updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var publisher = await _publisherService.GetByIdAsync(id);
        if (publisher is null)
            return NotFound();

        return View(publisher);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _publisherService.DeleteAsync(id);

        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = string.Join(" ", result.Errors);
            return RedirectToAction(nameof(Delete), new { id });
        }

        _logger.LogInformation("Publisher {PublisherId} deleted via UI", id);
        TempData["SuccessMessage"] = "Publisher was deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}