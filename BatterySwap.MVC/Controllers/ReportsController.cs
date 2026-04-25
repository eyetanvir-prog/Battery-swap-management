using BatterySwap.MVC.Models;
using BatterySwap.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BatterySwap.MVC.Controllers;

public class ReportsController(BatterySwapApiService apiService) : Controller
{
    public async Task<IActionResult> Summary(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("AuthToken")))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var role = HttpContext.Session.GetString("UserRole") ?? "Admin";
            var dashboard = await apiService.GetDashboardAsync(role, cancellationToken);
            var swaps = await apiService.GetSwapHistoryAsync(cancellationToken);
            var recharges = await apiService.GetRechargeHistoryAsync(cancellationToken);

            return View(new ReportSummaryViewModel
            {
                ScopeLabel = string.Equals(role, "Employee", StringComparison.OrdinalIgnoreCase)
                    ? "Employee Station Report"
                    : "System Summary Report",
                StationCount = dashboard.StationStocks.Count,
                TotalSwaps = swaps.Count,
                TotalSwapRevenue = SumMoney(swaps.Select(x => x.SwapCost)),
                TotalRecharges = recharges.Count,
                TotalRechargeAmount = SumMoney(recharges.Select(x => x.Amount)),
                LowAvailabilityStations = dashboard.StationStocks.Count(x => x.AvailableBatteries <= 2),
                StationStocks = dashboard.StationStocks,
                RecentSwaps = swaps.Take(8).ToList(),
                RecentRecharges = recharges.Take(8).ToList()
            });
        }
        catch (UnauthorizedAccessException)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }

    private static decimal SumMoney(IEnumerable<string> values)
    {
        return values.Sum(value =>
        {
            var raw = value.Replace(" Tk", string.Empty, StringComparison.OrdinalIgnoreCase);
            return decimal.TryParse(raw, out var parsed) ? parsed : 0m;
        });
    }
}
