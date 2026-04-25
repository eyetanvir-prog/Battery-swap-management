using BatterySwap.MVC.Models;
using BatterySwap.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BatterySwap.MVC.Controllers;

public class SwapController(BatterySwapApiService apiService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Process(string? phone, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("AuthToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!string.Equals(HttpContext.Session.GetString("UserRole"), "Employee", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction("Index", "Dashboard");
        }

        var model = new ProcessSwapViewModel
        {
            SearchPhone = phone ?? string.Empty
        };

        if (string.IsNullOrWhiteSpace(phone))
        {
            return View(model);
        }

        try
        {
            model.Client = await apiService.SearchClientByPhoneAsync(phone, cancellationToken);
            model.ClientId = model.Client.Id;
            model.AvailableBatteries = (await apiService.GetMyAvailableBatteriesAsync(cancellationToken)).ToList();
            if (model.AvailableBatteries.Count == 0)
            {
                model.ErrorMessage = "No available batteries were found at your station.";
            }
            return View(model);
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Process(ProcessSwapViewModel model, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("AuthToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!string.Equals(HttpContext.Session.GetString("UserRole"), "Employee", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction("Index", "Dashboard");
        }

        try
        {
            model.Client = await apiService.GetClientAsync(model.ClientId, cancellationToken);
            model.AvailableBatteries = (await apiService.GetMyAvailableBatteriesAsync(cancellationToken)).ToList();
            model.SearchPhone = model.Client.Phone;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await apiService.ProcessSwapAsync(model.ClientId, model.AssignedBatteryId, cancellationToken);
            TempData["SuccessMessage"] = $"Swap completed. New battery: {response.AssignedBatteryCode}. New balance: {response.NewBalance:0.##} Tk.";
            return RedirectToAction(nameof(Process), new { phone = model.SearchPhone });
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
