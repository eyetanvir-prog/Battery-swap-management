using BatterySwap.API.Data;
using BatterySwap.API.DTOs.Swaps;
using BatterySwap.API.Helpers;
using BatterySwap.API.Services;
using BatterySwap.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BatterySwap.API.Controllers;

[ApiController]
[Route("api/swaps")]
[Authorize(Roles = Roles.Admin + "," + Roles.Employee)]
public class SwapsController(AppDbContext dbContext, SwapService swapService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SwapHistoryItemResponse>>> GetHistory(CancellationToken cancellationToken)
    {
        var query =
            from swap in dbContext.Swaps.AsNoTracking()
            join client in dbContext.Clients.AsNoTracking() on swap.ClientId equals client.Id
            join station in dbContext.Stations.AsNoTracking() on swap.StationId equals station.Id
            join returnedBattery in dbContext.Batteries.AsNoTracking() on swap.ReturnedBatteryId equals returnedBattery.Id
            join assignedBattery in dbContext.Batteries.AsNoTracking() on swap.AssignedBatteryId equals assignedBattery.Id
            join employee in dbContext.Employees.AsNoTracking() on swap.ProcessedByEmployeeId equals employee.Id
            select new SwapHistoryItemResponse
            {
                Id = swap.Id,
                ClientId = client.Id,
                ClientName = client.Name,
                ClientPhone = client.Phone,
                StationName = station.StationName,
                ReturnedBatteryCode = returnedBattery.BatteryCode,
                AssignedBatteryCode = assignedBattery.BatteryCode,
                SwapCost = swap.SwapCost,
                SwapTime = swap.SwapTime,
                ProcessedByEmployee = employee.Name
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
            .OrderByDescending(x => x.SwapTime)
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("client/{clientId:int}")]
    public async Task<ActionResult<IReadOnlyList<SwapHistoryItemResponse>>> GetClientHistory(int clientId, CancellationToken cancellationToken)
    {
        var items = await
            (from swap in dbContext.Swaps.AsNoTracking()
             join client in dbContext.Clients.AsNoTracking() on swap.ClientId equals client.Id
             join station in dbContext.Stations.AsNoTracking() on swap.StationId equals station.Id
             join returnedBattery in dbContext.Batteries.AsNoTracking() on swap.ReturnedBatteryId equals returnedBattery.Id
             join assignedBattery in dbContext.Batteries.AsNoTracking() on swap.AssignedBatteryId equals assignedBattery.Id
             join employee in dbContext.Employees.AsNoTracking() on swap.ProcessedByEmployeeId equals employee.Id
             where swap.ClientId == clientId
             orderby swap.SwapTime descending
             select new SwapHistoryItemResponse
             {
                 Id = swap.Id,
                 ClientId = client.Id,
                 ClientName = client.Name,
                 ClientPhone = client.Phone,
                 StationName = station.StationName,
                 ReturnedBatteryCode = returnedBattery.BatteryCode,
                 AssignedBatteryCode = assignedBattery.BatteryCode,
                 SwapCost = swap.SwapCost,
                 SwapTime = swap.SwapTime,
                 ProcessedByEmployee = employee.Name
             })
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Employee)]
    public async Task<ActionResult<ProcessSwapResponse>> ProcessSwap([FromBody] ProcessSwapRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var stationId = User.GetStationId();
        var employeeId = User.GetUserId();

        if (!stationId.HasValue || !employeeId.HasValue)
        {
            return Forbid();
        }

        try
        {
            var response = await swapService.ProcessSwapAsync(
                request.ClientId,
                request.AssignedBatteryId,
                stationId.Value,
                employeeId.Value,
                cancellationToken);

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
