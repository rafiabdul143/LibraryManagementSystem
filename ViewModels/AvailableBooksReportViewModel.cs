using LibraryManagement.Web.Models.Entities;

namespace LibraryManagement.Web.ViewModels;

public class AvailableBooksReportViewModel
{
    public List<Book> Books { get; set; } = new();
    public string? SearchTerm { get; set; }
}