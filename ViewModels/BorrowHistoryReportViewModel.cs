using LibraryManagement.Web.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagement.Web.ViewModels;

public class BorrowHistoryReportViewModel
{
    public List<BorrowRecord> Records { get; set; } = new();

    public int? MemberId { get; set; }
    public int? BookId { get; set; }

    public List<SelectListItem> Members { get; set; } = new();
    public List<SelectListItem> Books { get; set; } = new();
}