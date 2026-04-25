using BatterySwap.API.Data;
using BatterySwap.API.DTOs.Stations;
using BatterySwap.API.Helpers;
using BatterySwap.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BatterySwap.API.Controllers;

[ApiController]
[Route("api/stations")]
[Authorize(Roles = Roles.Admin + "," + Roles.Employee)]
public class StationsController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StationListItemResponse>>> GetStations(CancellationToken cancellationToken)
    {
        var query = dbContext.Stations.AsNoTracking();

        if (User.IsInRole(Roles.Employee))
        {
            var stationId = User.GetStationId();
            if (!stationId.HasValue)
            {
                return Forbid();
            }

            query = query.Where(x => x.Id == stationId.Value);
        }

        var stations = await query
            .OrderBy(x => x.StationName)
            .Select(x => new StationListItemResponse
            {
                Id = x.Id,
                StationName = x.StationName,
                StationCode = x.StationCode,
                Address = x.Address ?? string.Empty,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                AvailableBatteries = dbContext.Batteries.Count(b => b.StationId == x.Id && b.Status == "Available"),
                TotalBatteries = dbContext.Batteries.Count(b => b.StationId == x.Id),
                EmployeeName = dbContext.Employees.Where(e => e.StationId == x.Id).Select(e => e.Name).FirstOrDefault() ?? "Unassigned",
                Status = x.Status
            })
            .ToListAsync(cancellationToken);

        return Ok(stations);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<StationListItemResponse>> GetStation(int id, CancellationToken cancellationToken)
    {
        var station = await dbContext.Stations
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new StationListItemResponse
            {
                Id = x.Id,
                StationName = x.StationName,
                StationCode = x.StationCode,
                Address = x.Address ?? string.Empty,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                AvailableBatteries = dbContext.Batteries.Count(b => b.StationId == x.Id && b.Status == "Available"),
                TotalBatteries = dbContext.Batteries.Count(b => b.StationId == x.Id),
                EmployeeName = dbContext.Employees.Where(e => e.StationId == x.Id).Select(e => e.Name).FirstOrDefault() ?? "Unassigned",
                Status = x.Status
            })
            .FirstOrDefaultAsync(cancellationToken);

        return station is null ? NotFound() : Ok(station);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> CreateStation([FromBody] CreateStationRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (await dbContext.Stations.AnyAsync(x => x.StationCode == request.StationCode.Trim(), cancellationToken))
        {
            return BadRequest(new { message = "Station code already exists." });
        }

        var station = new Models.Station
        {
            StationName = request.StationName.Trim(),
            StationCode = request.StationCode.Trim(),
            Address = request.Address?.Trim(),
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Status = "Open"
        };

        dbContext.Stations.Add(station);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Created($"/api/stations/{station.Id}", new { station.Id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UpdateStation(int id, [FromBody] UpdateStationRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var station = await dbContext.Stations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (station is null)
        {
            return NotFound();
        }

        if (await dbContext.Stations.AnyAsync(x => x.Id != id && x.StationCode == request.StationCode.Trim(), cancellationToken))
        {
            return BadRequest(new { message = "Station code already exists." });
        }

        station.StationName = request.StationName.Trim();
        station.StationCode = request.StationCode.Trim();
        station.Address = request.Address?.Trim();
        station.Latitude = request.Latitude;
        station.Longitude = request.Longitude;
        station.Status = request.Status.Trim();

        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("lookup")]
    public async Task<ActionResult<IReadOnlyList<StationLookupResponse>>> GetLookup(CancellationToken cancellationToken)
    {
        var stations = await dbContext.Stations
            .AsNoTracking()
            .OrderBy(x => x.StationName)
            .Select(x => new StationLookupResponse
            {
                Id = x.Id,
                StationName = x.StationName,
                StationCode = x.StationCode
            })
            .ToListAsync(cancellationToken);

        return Ok(stations);
    }
}
