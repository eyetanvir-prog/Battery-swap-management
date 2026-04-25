using System.Net.Http.Json;
using BatterySwap.MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace BatterySwap.MVC.Controllers;

public class AccountController(IHttpClientFactory httpClientFactory) : Controller
{
    [HttpGet]
    public IActionResult Login(string? reason = null, string? returnUrl = null)
    {
        if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString("AuthToken")))
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl,
            InfoMessage = reason switch
            {
                "session" => "Your session is missing or expired. Please sign in again to continue.",
                "logout" => "You have been signed out successfully.",
                _ => null
            }
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var client = httpClientFactory.CreateClient("BatterySwapApi");
            var response = await client.PostAsJsonAsync("/api/auth/login", new
            {
                model.UsernameOrEmail,
                model.Password
            }, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                model.ErrorMessage = "Login failed. Please check your credentials.";
                return View(model);
            }

            var authResponse = await response.Content.ReadFromJsonAsync<AuthLoginResponse>(cancellationToken: cancellationToken);
            if (authResponse is null || string.IsNullOrWhiteSpace(authResponse.Token))
            {
                model.ErrorMessage = "Login response was empty.";
                return View(model);
            }

            HttpContext.Session.SetString("AuthToken", authResponse.Token);
            HttpContext.Session.SetString("UserRole", authResponse.Role);
            HttpContext.Session.SetString("DisplayName", authResponse.DisplayName);
            var stationId = ReadStationIdFromJwt(authResponse.Token);
            if (stationId.HasValue)
            {
                HttpContext.Session.SetInt32("StationId", stationId.Value);
            }
            else
            {
                HttpContext.Session.Remove("StationId");
            }

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
        catch (HttpRequestException)
        {
            model.ErrorMessage = "The API could not be reached from the MVC app. Please check that the backend is running.";
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login), new { reason = "logout" });
    }

    private static int? ReadStationIdFromJwt(string token)
    {
        var parts = token.Split('.');
        if (parts.Length < 2)
        {
            return null;
        }

        var payload = parts[1]
            .Replace('-', '+')
            .Replace('_', '/');

        switch (payload.Length % 4)
        {
            case 2:
                payload += "==";
                break;
            case 3:
                payload += "=";
                break;
        }

        try
        {
            var jsonBytes = Convert.FromBase64String(payload);
            using var document = System.Text.Json.JsonDocument.Parse(jsonBytes);
            if (!document.RootElement.TryGetProperty("station_id", out var stationIdElement))
            {
                return null;
            }

            return stationIdElement.ValueKind switch
            {
                System.Text.Json.JsonValueKind.Number when stationIdElement.TryGetInt32(out var number) => number,
                System.Text.Json.JsonValueKind.String when int.TryParse(stationIdElement.GetString(), out var value) => value,
                _ => null
            };
        }
        catch
        {
            return null;
        }
    }
}
