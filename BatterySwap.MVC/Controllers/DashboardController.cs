using BatterySwap.MVC.Models;
using BatterySwap.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BatterySwap.MVC.Controllers;

public class DashboardController(BatterySwapApiService apiService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("AuthToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var role = HttpContext.Session.GetString("UserRole") ?? "Admin";
            var model = await apiService.GetDashboardAsync(role, cancellationToken);
            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
