using BatterySwap.MVC.Models;
using BatterySwap.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BatterySwap.MVC.Controllers;

public class ClientController(BatterySwapApiService apiService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("AuthToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var clients = await apiService.GetClientsAsync(cancellationToken);
            return View(new ClientListViewModel
            {
                Clients = clients
            });
        }
        catch (UnauthorizedAccessException)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Profile(int id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("AuthToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var client = await apiService.GetClientAsync(id, cancellationToken);
            var history = await apiService.GetClientSwapHistoryAsync(id, cancellationToken);

            return View(new ClientProfileViewModel
            {
                Client = client,
                SwapHistory = history
            });
        }
        catch (UnauthorizedAccessException)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Recharge(int id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("AuthToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var client = await apiService.GetClientAsync(id, cancellationToken);
            var role = HttpContext.Session.GetString("UserRole") ?? string.Empty;
            var stations = string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase)
                ? (await apiService.GetStationsAsync(cancellationToken)).ToList()
                : [];

            return View(new RechargeClientViewModel
            {
                ClientId = client.Id,
                ClientName = client.Name,
                Phone = client.Phone,
                CurrentBalance = client.BalanceDisplay,
                RequiresStationSelection = string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase),
                Stations = stations
            });
        }
        catch (UnauthorizedAccessException)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
        catch (HttpRequestException)
        {
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Recharge(RechargeClientViewModel model, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("AuthToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var client = await apiService.GetClientAsync(model.ClientId, cancellationToken);
            model.ClientName = client.Name;
            model.Phone = client.Phone;
            model.CurrentBalance = client.BalanceDisplay;
            model.RequiresStationSelection = string.Equals(HttpContext.Session.GetString("UserRole"), "Admin", StringComparison.OrdinalIgnoreCase);
            model.Stations = model.RequiresStationSelection
                ? (await apiService.GetStationsAsync(cancellationToken)).ToList()
                : [];

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await apiService.RechargeClientAsync(model, cancellationToken);
            TempData["SuccessMessage"] = $"Recharge successful for {model.ClientName}.";
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
    public IActionResult Create()
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("AuthToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        return View(new CreateClientViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateClientViewModel model, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("AuthToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await apiService.CreateClientAsync(model, cancellationToken);
            TempData["SuccessMessage"] = "Client registered successfully with 1,000 Tk starting balance.";
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

        try
        {
            var model = await apiService.GetClientForEditAsync(id, cancellationToken);
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
    public async Task<IActionResult> Edit(EditClientViewModel model, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("AuthToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await apiService.UpdateClientAsync(model, cancellationToken);
            TempData["SuccessMessage"] = "Client updated successfully.";
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("AuthToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            await apiService.DeactivateClientAsync(id, cancellationToken);
            TempData["SuccessMessage"] = "Client deactivated successfully.";
        }
        catch (UnauthorizedAccessException)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
        catch (HttpRequestException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
