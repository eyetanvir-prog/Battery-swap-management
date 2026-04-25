using BatterySwap.API.Data;
using BatterySwap.API.DTOs.Dashboard;
using BatterySwap.API.Helpers;
using BatterySwap.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BatterySwap.API.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet("admin")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<DashboardSummaryResponse>> GetAdminDashboard(CancellationToken cancellationToken)
    {
        var stationStocks = await GetStationStocksQuery().ToListAsync(cancellationToken);
        var recentActivities = await GetAdminActivityAsync(cancellationToken);
        var today = DateTime.UtcNow.Date;
        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var batteriesInUse = await dbContext.Batteries.CountAsync(x => x.CurrentClientId != null, cancellationToken);
        var lowBalanceClients = await dbContext.Clients.CountAsync(
            x => x.Status == "Active" && x.Balance < BatterySwapDefaults.SwapCost,
            cancellationToken);
        var todaysRevenue = await dbContext.Swaps
            .Where(x => x.SwapTime >= today)
            .SumAsync(x => (decimal?)x.SwapCost, cancellationToken) ?? 0m;
        var monthRechargeVolume = await dbContext.BalanceRecharges
            .Where(x => x.RechargeTime >= monthStart)
            .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;

        var response = new DashboardSummaryResponse
        {
            StatCards =
            [
                new() { Title = "Stations", Value = (await dbContext.Stations.CountAsync(cancellationToken)).ToString(), Subtitle = "Connected network", AccentClass = "text-bg-primary", IconClass = "bi bi-geo-alt" },
                new() { Title = "Employees", Value = (await dbContext.Employees.CountAsync(cancellationToken)).ToString(), Subtitle = "Registered operators", AccentClass = "text-bg-success", IconClass = "bi bi-person-badge" },
                new() { Title = "Clients", Value = (await dbContext.Clients.CountAsync(cancellationToken)).ToString(), Subtitle = "Registered drivers", AccentClass = "text-bg-warning", IconClass = "bi bi-people" },
                new() { Title = "Today's Swaps", Value = (await dbContext.Swaps.CountAsync(x => x.SwapTime >= today, cancellationToken)).ToString(), Subtitle = "Processed today", AccentClass = "text-bg-danger", IconClass = "bi bi-battery-charging" }
            ],
            Highlights =
            [
                new() { Title = "Batteries In Use", Value = batteriesInUse.ToString(), Subtitle = "Currently with clients", AccentClass = "text-bg-dark", IconClass = "bi bi-lightning-charge" },
                new() { Title = "Low Balance Clients", Value = lowBalanceClients.ToString(), Subtitle = "Need recharge before next swap", AccentClass = "text-bg-warning", IconClass = "bi bi-exclamation-triangle" },
                new() { Title = "Today's Revenue", Value = $"{todaysRevenue:0.##} Tk", Subtitle = "Swap earnings today", AccentClass = "text-bg-info", IconClass = "bi bi-cash-stack" },
                new() { Title = "Month Recharge", Value = $"{monthRechargeVolume:0.##} Tk", Subtitle = "Wallet top-ups this month", AccentClass = "text-bg-primary", IconClass = "bi bi-wallet2" }
            ],
            RecentActivities = recentActivities,
            StationStocks = stationStocks
        };

        return Ok(response);
    }

    [HttpGet("employee")]
    [Authorize(Roles = Roles.Employee)]
    public async Task<ActionResult<DashboardSummaryResponse>> GetEmployeeDashboard(CancellationToken cancellationToken)
    {
        var stationId = User.GetStationId();
        if (!stationId.HasValue)
        {
            return Forbid();
        }

        var employeeId = User.GetUserId() ?? 0;

        var stationCode = await dbContext.Stations
            .Where(x => x.Id == stationId.Value)
            .Select(x => x.StationCode)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(stationCode))
        {
            return Forbid();
        }

        var stationStocks = await GetStationStocksQuery()
            .Where(x => x.StationCode == stationCode)
            .ToListAsync(cancellationToken);

        var availableBatteries = await dbContext.Batteries.CountAsync(x => x.StationId == stationId.Value && x.Status == "Available", cancellationToken);
        var totalBatteries = await dbContext.Batteries.CountAsync(x => x.StationId == stationId.Value, cancellationToken);
        var activeClients = await dbContext.Batteries.CountAsync(x => x.StationId == stationId.Value && x.CurrentClientId != null, cancellationToken);
        var lowBalanceHolders = await (
            from battery in dbContext.Batteries.AsNoTracking()
            join client in dbContext.Clients.AsNoTracking() on battery.CurrentClientId equals client.Id
            where battery.StationId == stationId.Value && client.Balance < BatterySwapDefaults.SwapCost && client.Status == "Active"
            select client.Id)
            .Distinct()
            .CountAsync(cancellationToken);
        var today = DateTime.UtcNow.Date;
        var todaysSwaps = await dbContext.Swaps.CountAsync(x => x.StationId == stationId.Value && x.SwapTime >= today, cancellationToken);
        var todaysRevenue = await dbContext.Swaps
            .Where(x => x.StationId == stationId.Value && x.SwapTime >= today)
            .SumAsync(x => (decimal?)x.SwapCost, cancellationToken) ?? 0m;
        var todaysRecharge = await dbContext.BalanceRecharges
            .Where(x => x.StationId == stationId.Value && x.RechargeTime >= today)
            .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;

        var recentActivities = await GetEmployeeActivityAsync(stationId.Value, employeeId, cancellationToken);

        var response = new DashboardSummaryResponse
        {
            StatCards =
            [
                new() { Title = "My Station", Value = "1", Subtitle = "Assigned location", AccentClass = "text-bg-primary", IconClass = "bi bi-geo-alt" },
                new() { Title = "Available Batteries", Value = availableBatteries.ToString(), Subtitle = "Ready to swap", AccentClass = "text-bg-success", IconClass = "bi bi-battery-charging" },
                new() { Title = "Active Clients", Value = activeClients.ToString(), Subtitle = "Holding station batteries", AccentClass = "text-bg-warning", IconClass = "bi bi-people" },
                new() { Title = "Today's Swaps", Value = todaysSwaps.ToString(), Subtitle = $"From {totalBatteries} total batteries", AccentClass = "text-bg-danger", IconClass = "bi bi-arrow-repeat" }
            ],
            Highlights =
            [
                new() { Title = "Batteries In Use", Value = activeClients.ToString(), Subtitle = "Checked out from your station", AccentClass = "text-bg-dark", IconClass = "bi bi-lightning-charge" },
                new() { Title = "Low Balance Holders", Value = lowBalanceHolders.ToString(), Subtitle = "May need recharge soon", AccentClass = "text-bg-warning", IconClass = "bi bi-exclamation-triangle" },
                new() { Title = "Today's Revenue", Value = $"{todaysRevenue:0.##} Tk", Subtitle = "Swap earnings at your desk", AccentClass = "text-bg-info", IconClass = "bi bi-cash-stack" },
                new() { Title = "Today's Recharge", Value = $"{todaysRecharge:0.##} Tk", Subtitle = "Wallet top-ups today", AccentClass = "text-bg-primary", IconClass = "bi bi-wallet2" }
            ],
            RecentActivities = recentActivities,
            StationStocks = stationStocks
        };

        return Ok(response);
    }

    private IQueryable<DashboardStationStockResponse> GetStationStocksQuery()
    {
        return dbContext.Stations
            .AsNoTracking()
            .Select(station => new DashboardStationStockResponse
            {
                StationName = station.StationName,
                StationCode = station.StationCode,
                AvailableBatteries = dbContext.Batteries.Count(b => b.StationId == station.Id && b.Status == "Available"),
                TotalBatteries = dbContext.Batteries.Count(b => b.StationId == station.Id),
                EmployeeName = dbContext.Employees.Where(e => e.StationId == station.Id).Select(e => e.Name).FirstOrDefault() ?? "Unassigned",
                Status = station.Status
            })
            .OrderBy(x => x.StationName);
    }

    private async Task<List<DashboardActivityItemResponse>> GetAdminActivityAsync(CancellationToken cancellationToken)
    {
        var clientActivities = await dbContext.Clients.AsNoTracking()
            .OrderByDescending(x => x.RegistrationDate)
            .Take(3)
            .Select(x => new DashboardActivityItemResponse
            {
                Title = "New client registered",
                Detail = $"{x.Name} joined with {x.VehicleType ?? "vehicle"} {x.VehicleNumber ?? string.Empty}".Trim(),
                TimestampUtc = x.RegistrationDate,
                BadgeClass = "text-bg-warning",
                BadgeText = "Client"
            })
            .ToListAsync(cancellationToken);

        var rechargeActivities = await dbContext.BalanceRecharges.AsNoTracking()
            .OrderByDescending(x => x.RechargeTime)
            .Take(2)
            .Select(x => new DashboardActivityItemResponse
            {
                Title = "Wallet recharge added",
                Detail = $"{x.Amount:0.##} Tk added to client #{x.ClientId} at station #{x.StationId}",
                TimestampUtc = x.RechargeTime,
                BadgeClass = "text-bg-primary",
                BadgeText = "Recharge"
            })
            .ToListAsync(cancellationToken);

        return clientActivities
            .Concat(rechargeActivities)
            .OrderByDescending(x => x.TimestampUtc)
            .Take(5)
            .ToList();
    }

    private async Task<List<DashboardActivityItemResponse>> GetEmployeeActivityAsync(int stationId, int employeeId, CancellationToken cancellationToken)
    {
        var clientActivities = await dbContext.Clients.AsNoTracking()
            .Where(x => x.CreatedByEmployeeId == employeeId)
            .OrderByDescending(x => x.RegistrationDate)
            .Take(3)
            .Select(x => new DashboardActivityItemResponse
            {
                Title = "Client registered",
                Detail = $"{x.Name} was added from your station workflow.",
                TimestampUtc = x.RegistrationDate,
                BadgeClass = "text-bg-warning",
                BadgeText = "Client"
            })
            .ToListAsync(cancellationToken);

        var rechargeActivities = await dbContext.BalanceRecharges.AsNoTracking()
            .Where(x => x.StationId == stationId)
            .OrderByDescending(x => x.RechargeTime)
            .Take(2)
            .Select(x => new DashboardActivityItemResponse
            {
                Title = "Recharge recorded",
                Detail = $"{x.Amount:0.##} Tk added at your station.",
                TimestampUtc = x.RechargeTime,
                BadgeClass = "text-bg-primary",
                BadgeText = "Recharge"
            })
            .ToListAsync(cancellationToken);

        return clientActivities
            .Concat(rechargeActivities)
            .OrderByDescending(x => x.TimestampUtc)
            .Take(5)
            .ToList();
    }
}
