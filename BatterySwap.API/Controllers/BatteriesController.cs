using BatterySwap.API.Data;
using BatterySwap.API.DTOs.Batteries;
using BatterySwap.API.Helpers;
using BatterySwap.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BatterySwap.API.Controllers;

[ApiController]
[Route("api/batteries")]
[Authorize(Roles = Roles.Admin + "," + Roles.Employee)]
public class BatteriesController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BatteryListItemResponse>>> GetBatteries(CancellationToken cancellationToken)
    {
        var query =
            from battery in dbContext.Batteries.AsNoTracking()
            join station in dbContext.Stations.AsNoTracking() on battery.StationId equals station.Id
            join client in dbContext.Clients.AsNoTracking() on battery.CurrentClientId equals client.Id into clientGroup
            from client in clientGroup.DefaultIfEmpty()
            select new BatteryListItemResponse
            {
                Id = battery.Id,
                BatteryCode = battery.BatteryCode,
                StationId = station.Id,
                StationName = station.StationName,
                Status = battery.Status,
                CurrentClientId = battery.CurrentClientId,
                CurrentClientName = client != null ? client.Name : "At station",
                LastUpdated = battery.LastUpdated
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

        var batteries = await query
            .OrderBy(x => x.BatteryCode)
            .ToListAsync(cancellationToken);

        return Ok(batteries);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<BatteryDetailResponse>> GetBattery(int id, CancellationToken cancellationToken)
    {
        var battery = await
            (from item in dbContext.Batteries.AsNoTracking()
             join station in dbContext.Stations.AsNoTracking() on item.StationId equals station.Id
             join client in dbContext.Clients.AsNoTracking() on item.CurrentClientId equals client.Id into clientGroup
             from client in clientGroup.DefaultIfEmpty()
             where item.Id == id
             select new BatteryDetailResponse
             {
                 Id = item.Id,
                 BatteryCode = item.BatteryCode,
                 StationId = item.StationId,
                 StationName = station.StationName,
                 Status = item.Status,
                 CurrentClientId = item.CurrentClientId,
                 CurrentClientName = client != null ? client.Name : "At station",
                 LastUpdated = item.LastUpdated
             })
            .FirstOrDefaultAsync(cancellationToken);

        return battery is null ? NotFound() : Ok(battery);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> CreateBattery([FromBody] CreateBatteryRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var stationExists = await dbContext.Stations.AnyAsync(x => x.Id == request.StationId, cancellationToken);
        if (!stationExists)
        {
            return BadRequest(new { message = "Selected station was not found." });
        }

        if (await dbContext.Batteries.AnyAsync(x => x.BatteryCode == request.BatteryCode.Trim(), cancellationToken))
        {
            return BadRequest(new { message = "Battery code already exists." });
        }

        var battery = new Models.Battery
        {
            BatteryCode = request.BatteryCode.Trim(),
            StationId = request.StationId,
            Status = "Available",
            LastUpdated = DateTime.UtcNow
        };

        dbContext.Batteries.Add(battery);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Created($"/api/batteries/{battery.Id}", new { battery.Id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UpdateBattery(int id, [FromBody] UpdateBatteryRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var battery = await dbContext.Batteries.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (battery is null)
        {
            return NotFound();
        }

        if (await dbContext.Batteries.AnyAsync(x => x.Id != id && x.BatteryCode == request.BatteryCode.Trim(), cancellationToken))
        {
            return BadRequest(new { message = "Battery code already exists." });
        }

        var stationExists = await dbContext.Stations.AnyAsync(x => x.Id == request.StationId, cancellationToken);
        if (!stationExists)
        {
            return BadRequest(new { message = "Selected station was not found." });
        }

        if (battery.CurrentClientId.HasValue && battery.StationId != request.StationId)
        {
            return BadRequest(new { message = "A battery with a client cannot be moved to another station." });
        }

        battery.BatteryCode = request.BatteryCode.Trim();
        battery.StationId = request.StationId;
        battery.LastUpdated = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("available/{stationId:int}")]
    public async Task<ActionResult<IReadOnlyList<BatteryLookupResponse>>> GetAvailable(int stationId, CancellationToken cancellationToken)
    {
        if (User.IsInRole(Roles.Employee))
        {
            var employeeStationId = User.GetStationId();
            if (!employeeStationId.HasValue || employeeStationId.Value != stationId)
            {
                return Forbid();
            }
        }

        var batteries = await dbContext.Batteries
            .AsNoTracking()
            .Where(x => x.StationId == stationId && x.Status == "Available")
            .OrderBy(x => x.BatteryCode)
            .Select(x => new BatteryLookupResponse
            {
                Id = x.Id,
                BatteryCode = x.BatteryCode
            })
            .ToListAsync(cancellationToken);

        return Ok(batteries);
    }
}
