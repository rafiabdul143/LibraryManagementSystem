namespace LibraryManagement.Web.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    /// <summary>Optional human-readable message set by GlobalExceptionMiddleware
    /// or the exception-handler pipeline for display to the user.</summary>
    public string? Message { get; set; }
}