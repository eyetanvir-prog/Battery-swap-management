using BatterySwap.API.Data;
using BatterySwap.API.DTOs.Recharges;
using BatterySwap.API.Helpers;
using BatterySwap.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BatterySwap.API.Controllers;

[ApiController]
[Route("api/recharges")]
[Authorize(Roles = Roles.Admin + "," + Roles.Employee)]
public class RechargesController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RechargeHistoryItemResponse>>> GetHistory(CancellationToken cancellationToken)
    {
        var query =
            from recharge in dbContext.BalanceRecharges.AsNoTracking()
            join client in dbContext.Clients.AsNoTracking() on recharge.ClientId equals client.Id
            join station in dbContext.Stations.AsNoTracking() on recharge.StationId equals station.Id
            join admin in dbContext.Admins.AsNoTracking() on recharge.AddedByAdminId equals admin.Id into adminGroup
            from admin in adminGroup.DefaultIfEmpty()
            join employee in dbContext.Employees.AsNoTracking() on recharge.AddedByEmployeeId equals employee.Id into employeeGroup
            from employee in employeeGroup.DefaultIfEmpty()
            select new RechargeHistoryItemResponse
            {
                Id = recharge.Id,
                ClientId = client.Id,
                ClientName = client.Name,
                ClientPhone = client.Phone,
                StationName = station.StationName,
                Amount = recharge.Amount,
                RechargeTime = recharge.RechargeTime,
                AddedBy = admin != null ? admin.Name : employee != null ? employee.Name : "Unknown"
            };

        if (User.IsInRole(Roles.Employee))
        {
            var stationId = User.GetStationId();
            if (!stationId.HasValue)
            {
                return Forbid();
            }

            query = query.Where(x => x.StationName == dbContext.Stations.Where(s => s.Id == stationId.Value).Select(s => s.StationName).First());
        }

        var items = await query
            .OrderByDescending(x => x.RechargeTime)
            .ToListAsync(cancellationToken);

        return Ok(items);
    }
}
