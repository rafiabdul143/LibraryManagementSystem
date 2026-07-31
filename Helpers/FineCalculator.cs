
namespace LibraryManagement.Web.Helpers;

/// <summary>
/// Pure calculation helper - no DB/service dependencies, easy to unit test.
/// </summary>
public static class FineCalculator
{
    /// <summary>
    /// Returns 0 if returned on/before the due date; otherwise
    /// (days late) x finePerDay. Compares dates only (ignores time-of-day)
    /// so a same-day-but-later-hour return isn't charged a partial day.
    /// </summary>
    public static decimal Calculate(DateTime dueDate, DateTime returnDate, decimal finePerDay)
    {
        if (returnDate.Date <= dueDate.Date)
            return 0m;

        var daysLate = (returnDate.Date - dueDate.Date).Days;
        return daysLate * finePerDay;
    }
}