namespace LibraryManagement.Web.Models;

/// <summary>
/// Strongly-typed representation of the "AppSettings" section in appsettings.json.
/// Bound via builder.Services.Configure&lt;AppSettings&gt;(...) in Program.cs and
/// consumed elsewhere through IOptions&lt;AppSettings&gt; / IOptionsSnapshot&lt;AppSettings&gt;.
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Number of days a member may keep a book before it is considered overdue.
    /// Used by BorrowService when calculating the DueDate on issue.
    /// </summary>
    public int BorrowPeriodDays { get; set; } = 14;

    /// <summary>
    /// Fine charged per day (in the local currency) for each day a book is
    /// returned after its DueDate. Used by FineCalculator.
    /// </summary>
    public decimal FinePerDayAmount { get; set; } = 5.00m;

    /// <summary>
    /// Email address used to seed the default Administrator account on first run.
    /// </summary>
    public string DefaultAdminEmail { get; set; } = "admin@library.local";

    /// <summary>
    /// Password used to seed the default Administrator account on first run.
    /// Must satisfy the Identity password policy configured in Program.cs.
    /// </summary>
    public string DefaultAdminPassword { get; set; } = "Admin@12345";
}