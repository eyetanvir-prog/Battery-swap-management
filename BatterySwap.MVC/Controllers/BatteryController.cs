using BatterySwap.MVC.Models;
using BatterySwap.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BatterySwap.MVC.Controllers;

public class BatteryController(BatterySwapApiService apiService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("AuthToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var batteries = await apiService.GetBatteryListAsync(cancellationToken);
            return View(new BatteryListViewModel
            {
                Batteries = batteries
            });
        }
        catch (UnauthorizedAccessException)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("AuthToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!string.Equals(HttpContext.Session.GetString("UserRole"), "Admin", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var stations = await apiService.GetStationsAsync(cancellationToken);
            return View(new CreateBatteryViewModel
            {
                Stations = stations.ToList()
            });
        }
        catch (UnauthorizedAccessException)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBatteryViewModel model, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("AuthToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!string.Equals(HttpContext.Session.GetString("UserRole"), "Admin", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            model.Stations = (await apiService.GetStationsAsync(cancellationToken)).ToList();
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await apiService.CreateBatteryAsync(model, cancellationToken);
            TempData["SuccessMessage"] = "Battery created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (UnauthorizedAccessException)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
        catch (HttpRequestException ex)
        {
            model.ErrorMessage = ex.Message;
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("AuthToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!string.Equals(HttpContext.Session.GetString("UserRole"), "Admin", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var model = await apiService.GetBatteryForEditAsync(id, cancellationToken);
            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditBatteryViewModel model, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("AuthToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!string.Equals(HttpContext.Session.GetString("UserRole"), "Admin", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            model.Stations = (await apiService.GetStationsAsync(cancellationToken)).ToList();
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await apiService.UpdateBatteryAsync(model, cancellationToken);
            TempData["SuccessMessage"] = "Battery updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (UnauthorizedAccessException)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
        catch (HttpRequestException ex)
        {
            model.ErrorMessage = ex.Message;
            return View(model);
        }
    }
}
