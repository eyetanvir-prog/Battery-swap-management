using BatterySwap.API.Data;
using BatterySwap.API.DTOs.Employees;
using BatterySwap.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BatterySwap.API.Controllers;

[ApiController]
[Route("api/employees")]
[Authorize(Roles = Roles.Admin)]
public class EmployeesController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmployeeListItemResponse>>> GetEmployees(CancellationToken cancellationToken)
    {
        var employees = await
            (from employee in dbContext.Employees.AsNoTracking()
             join station in dbContext.Stations.AsNoTracking()
                 on employee.StationId equals station.Id into stationGroup
             from station in stationGroup.DefaultIfEmpty()
             orderby employee.Name
             select new EmployeeListItemResponse
             {
                 Id = employee.Id,
                 Name = employee.Name,
                 Username = employee.Username,
                 Phone = employee.Phone,
                 Nid = employee.Nid,
                 StationName = station != null ? station.StationName : "Unassigned",
                 Status = employee.Status,
                 JoiningDate = employee.JoiningDate
             })
            .ToListAsync(cancellationToken);

        return Ok(employees);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeDetailResponse>> GetEmployee(int id, CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new EmployeeDetailResponse
            {
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                Nid = x.Nid,
                Address = x.Address ?? string.Empty,
                StationId = x.StationId,
                Username = x.Username,
                Status = x.Status,
                JoiningDate = x.JoiningDate
            })
            .FirstOrDefaultAsync(cancellationToken);

        return employee is null ? NotFound() : Ok(employee);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var station = await dbContext.Stations.FirstOrDefaultAsync(x => x.Id == request.StationId, cancellationToken);
        if (station is null)
        {
            return BadRequest(new { message = "Selected station was not found." });
        }

        if (station.EmployeeId.HasValue)
        {
            return BadRequest(new { message = "The selected station already has an assigned employee." });
        }

        if (await dbContext.Employees.AnyAsync(x => x.Phone == request.Phone, cancellationToken))
        {
            return BadRequest(new { message = "Phone number already exists." });
        }

        if (await dbContext.Employees.AnyAsync(x => x.Nid == request.Nid, cancellationToken))
        {
            return BadRequest(new { message = "NID already exists." });
        }

        if (await dbContext.Employees.AnyAsync(x => x.Username == request.Username, cancellationToken))
        {
            return BadRequest(new { message = "Username already exists." });
        }

        var employee = new Models.Employee
        {
            Name = request.Name.Trim(),
            Phone = request.Phone.Trim(),
            Nid = request.Nid.Trim(),
            Address = request.Address?.Trim(),
            StationId = request.StationId,
            Username = request.Username.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Status = "Active",
            JoiningDate = request.JoiningDate ?? DateOnly.FromDateTime(DateTime.Today)
        };

        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync(cancellationToken);

        station.EmployeeId = employee.Id;
        await dbContext.SaveChangesAsync(cancellationToken);

        return Created($"/api/employees/{employee.Id}", new { employee.Id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var employee = await dbContext.Employees.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        if (await dbContext.Employees.AnyAsync(x => x.Id != id && x.Phone == request.Phone, cancellationToken))
        {
            return BadRequest(new { message = "Phone number already exists." });
        }

        if (await dbContext.Employees.AnyAsync(x => x.Id != id && x.Nid == request.Nid, cancellationToken))
        {
            return BadRequest(new { message = "NID already exists." });
        }

        if (await dbContext.Employees.AnyAsync(x => x.Id != id && x.Username == request.Username, cancellationToken))
        {
            return BadRequest(new { message = "Username already exists." });
        }

        if (request.StationId.HasValue)
        {
            var station = await dbContext.Stations.FirstOrDefaultAsync(x => x.Id == request.StationId.Value, cancellationToken);
            if (station is null)
            {
                return BadRequest(new { message = "Selected station was not found." });
            }

            var assignedToAnother = await dbContext.Employees.AnyAsync(
                x => x.Id != id && x.StationId == request.StationId.Value,
                cancellationToken);

            if (assignedToAnother)
            {
                return BadRequest(new { message = "The selected station already has another assigned employee." });
            }
        }

        var previousStationId = employee.StationId;

        employee.Name = request.Name.Trim();
        employee.Phone = request.Phone.Trim();
        employee.Nid = request.Nid.Trim();
        employee.Address = request.Address?.Trim();
        employee.StationId = request.StationId;
        employee.Username = request.Username.Trim();
        employee.Status = request.Status.Trim();
        employee.JoiningDate = request.JoiningDate;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        }

        if (previousStationId.HasValue && previousStationId != request.StationId)
        {
            var previousStation = await dbContext.Stations.FirstOrDefaultAsync(x => x.Id == previousStationId.Value, cancellationToken);
            if (previousStation is not null && previousStation.EmployeeId == employee.Id)
            {
                previousStation.EmployeeId = null;
            }
        }

        if (request.StationId.HasValue)
        {
            var newStation = await dbContext.Stations.FirstOrDefaultAsync(x => x.Id == request.StationId.Value, cancellationToken);
            if (newStation is not null)
            {
                newStation.EmployeeId = employee.Id;
            }
        }

        if (!string.Equals(employee.Status, "Active", StringComparison.OrdinalIgnoreCase))
        {
            if (employee.StationId.HasValue)
            {
                var station = await dbContext.Stations.FirstOrDefaultAsync(x => x.Id == employee.StationId.Value, cancellationToken);
                if (station is not null && station.EmployeeId == employee.Id)
                {
                    station.EmployeeId = null;
                }
            }

            employee.StationId = null;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeactivateEmployee(int id, CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        employee.Status = "Inactive";

        if (employee.StationId.HasValue)
        {
            var station = await dbContext.Stations.FirstOrDefaultAsync(x => x.Id == employee.StationId.Value, cancellationToken);
            if (station is not null && station.EmployeeId == employee.Id)
            {
                station.EmployeeId = null;
            }
        }

        employee.StationId = null;
        await dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
