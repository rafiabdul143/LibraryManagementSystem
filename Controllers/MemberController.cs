using LibraryManagement.Web.Models.Entities;
using LibraryManagement.Web.Services.Interfaces;
using LibraryManagement.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Web.Controllers;

[Authorize]
public class MembersController : Controller
{
    private readonly IMemberService _memberService;
    private readonly ILogger<MembersController> _logger;

    public MembersController(IMemberService memberService, ILogger<MembersController> logger)
    {
        _memberService = memberService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var members = (await _memberService.GetAllAsync()).OrderBy(m => m.FullName).ToList();
        return View(members);
    }

    // Details shows full borrow history via GetWithHistoryAsync (built in Module 4).
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var member = await _memberService.GetWithHistoryAsync(id);
        if (member is null)
            return NotFound();

        return View(member);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new MemberViewModel());

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MemberViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var member = new Member
        {
            FullName = model.FullName,
            Email = model.Email,
            Phone = model.Phone,
            MembershipDate = DateTime.UtcNow
        };

        var result = await _memberService.CreateAsync(member);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);
            return View(model);
        }

        _logger.LogInformation("Member '{Email}' created via UI", member.Email);
        TempData["SuccessMessage"] = $"Member '{member.FullName}' was created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var member = await _memberService.GetByIdAsync(id);
        if (member is null)
            return NotFound();

        var model = new MemberViewModel
        {
            MemberId = member.MemberId,
            FullName = member.FullName,
            Email = member.Email,
            Phone = member.Phone
        };

        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MemberViewModel model)
    {
        if (id != model.MemberId)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        var member = new Member
        {
            MemberId = model.MemberId,
            FullName = model.FullName,
            Email = model.Email,
            Phone = model.Phone
        };

        var result = await _memberService.UpdateAsync(member);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);
            return View(model);
        }

        _logger.LogInformation("Member {MemberId} updated via UI", model.MemberId);
        TempData["SuccessMessage"] = $"Member '{member.FullName}' was updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var member = await _memberService.GetByIdAsync(id);
        if (member is null)
            return NotFound();

        return View(member);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _memberService.DeleteAsync(id);

        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = string.Join(" ", result.Errors);
            return RedirectToAction(nameof(Delete), new { id });
        }

        _logger.LogInformation("Member {MemberId} deleted via UI", id);
        TempData["SuccessMessage"] = "Member was deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}