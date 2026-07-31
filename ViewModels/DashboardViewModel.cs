using LibraryManagement.Web.Models.Entities;
using LibraryManagement.Web.Services;

namespace LibraryManagement.Web.ViewModels;

/// <summary>Aggregates everything the Dashboard/Index view needs in one payload.</summary>
public class DashboardViewModel
{
    public DashboardSummary Summary { get; set; } = null!;
    public List<CategoryBookCount> CategoryDistribution { get; set; } = new();
    public List<BorrowRecord> RecentActivity { get; set; } = new();
}