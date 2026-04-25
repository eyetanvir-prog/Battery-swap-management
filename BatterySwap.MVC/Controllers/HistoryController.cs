using BatterySwap.MVC.Models;
using BatterySwap.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BatterySwap.MVC.Controllers;

public class HistoryController(BatterySwapApiService apiService) : Controller
{
    public async Task<IActionResult> Swaps(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("AuthToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var items = await apiService.GetSwapHistoryAsync(cancellationToken);
            return View(new SwapHistoryListViewModel
            {
                Items = items
            });
        }
        catch (UnauthorizedAccessException)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }

    public async Task<IActionResult> Recharges(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("AuthToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var items = await apiService.GetRechargeHistoryAsync(cancellationToken);
            return View(new RechargeHistoryListViewModel
            {
                Items = items
            });
        }
        catch (UnauthorizedAccessException)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
