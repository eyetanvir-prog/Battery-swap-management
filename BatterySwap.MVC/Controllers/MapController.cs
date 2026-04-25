using BatterySwap.MVC.Models;
using BatterySwap.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BatterySwap.MVC.Controllers;

public class MapController(BatterySwapApiService apiService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("AuthToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var stations = await apiService.GetStationsListAsync(cancellationToken);
            return View(new StationListViewModel
            {
                Stations = stations
            });
        }
        catch (UnauthorizedAccessException)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
