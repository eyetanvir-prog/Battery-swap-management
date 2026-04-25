using System.Diagnostics;
using BatterySwap.MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace BatterySwap.MVC.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Dashboard");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View("Error", new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            StatusCode = 500,
            PageTitle = "System Error",
            Headline = "Something went wrong in the application.",
            Description = "The request could not be completed. Please return to the dashboard or try the previous action again.",
            ActionLabel = "Back To Dashboard",
            ActionUrl = Url.Action("Index", "Dashboard")
        });
    }

    [Route("Home/StatusCode")]
    public IActionResult StatusCodePage(int code)
    {
        var model = code switch
        {
            403 => new ErrorViewModel
            {
                StatusCode = code,
                PageTitle = "Access Denied",
                Headline = "You do not have permission to open this page.",
                Description = "This feature is restricted by role. Please sign in with an authorized account or return to the dashboard.",
                ActionLabel = "Go To Dashboard",
                ActionUrl = Url.Action("Index", "Dashboard")
            },
            404 => new ErrorViewModel
            {
                StatusCode = code,
                PageTitle = "Page Not Found",
                Headline = "The requested page could not be found.",
                Description = "The address may be incorrect or the page may have been moved during development.",
                ActionLabel = "Open Dashboard",
                ActionUrl = Url.Action("Index", "Dashboard")
            },
            _ => new ErrorViewModel
            {
                StatusCode = code,
                PageTitle = "Request Problem",
                Headline = "The request could not be completed.",
                Description = "Please return to the dashboard and try again. If the problem continues, refresh the session and repeat the action.",
                ActionLabel = "Back To Dashboard",
                ActionUrl = Url.Action("Index", "Dashboard")
            }
        };

        Response.StatusCode = code;
        return View("Error", model);
    }
}
