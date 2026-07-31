using LibraryManagement.Web.Services;

namespace LibraryManagement.Web.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummary> GetSummaryAsync();

    /// <summary>Book title count grouped by category, for the dashboard chart.</summary>
    Task<IEnumerable<CategoryBookCount>> GetCategoryDistributionAsync();
}