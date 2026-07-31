using LibraryManagement.Web.Services.Interfaces;
using LibraryManagement.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly IBorrowService _borrowService;

    public DashboardController(IDashboardService dashboardService, IBorrowService borrowService)
    {
        _dashboardService = dashboardService;
        _borrowService = borrowService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var summary = await _dashboardService.GetSummaryAsync();
        var distribution = (await _dashboardService.GetCategoryDistributionAsync()).ToList();

        // GetHistoryAsync(null, null) returns ALL borrow records (any status),
        // newest first, with Book and Member included - take the top 10 for the feed.
        var recentActivity = (await _borrowService.GetHistoryAsync(null, null))
            .Take(10)
            .ToList();

        var model = new DashboardViewModel
        {
            Summary = summary,
            CategoryDistribution = distribution,
            RecentActivity = recentActivity
        };

        return View(model);
    }
}