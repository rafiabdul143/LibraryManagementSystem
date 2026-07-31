namespace LibraryManagement.Web.Services;

/// <summary>
/// Aggregated counters for the dashboard cards. Kept intentionally minimal
/// here in Module 4 - Module 9 (Dashboard) builds a richer DashboardViewModel
/// (charts, recent activity) on top of this summary.
/// </summary>
public record DashboardSummary(
    int TotalBooks,
    int TotalMembers,
    int AvailableBooks,
    int BorrowedBooks,
    int OverdueBooks);