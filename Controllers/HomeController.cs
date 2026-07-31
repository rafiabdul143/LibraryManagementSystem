using System.Diagnostics;
using LibraryManagement.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Web.Controllers;

/// <summary>
/// Hosts the friendly error page. Reached two ways: app.UseExceptionHandler
/// ("/Home/Error") in production for exceptions caught inside the MVC
/// pipeline, or a redirect from GlobalExceptionMiddleware for exceptions
/// caught outside it (including all exceptions in Development, since no
/// UseDeveloperExceptionPage/UseExceptionHandler is registered there).
/// </summary>
public class HomeController : Controller
{
    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(string? requestId)
    {
        // Prefer the requestId passed by GlobalExceptionMiddleware (the ID of
        // the ORIGINAL failing request) over recomputing one for this fresh
        // /Home/Error request, which would be a different, misleading ID.
        var resolvedRequestId = !string.IsNullOrEmpty(requestId)
            ? requestId
            : (Activity.Current?.Id ?? HttpContext.TraceIdentifier);

        var model = new ErrorViewModel
        {
            RequestId = resolvedRequestId,
            Message = "Something went wrong while processing your request. The issue has been logged."
        };

        return View(model);
    }
}