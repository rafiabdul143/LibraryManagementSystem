namespace LibraryManagement.Web.Services;

/// <summary>Number of book titles catalogued under a given category - powers the
/// Dashboard's category-distribution chart.</summary>
public record CategoryBookCount(string CategoryName, int BookCount);