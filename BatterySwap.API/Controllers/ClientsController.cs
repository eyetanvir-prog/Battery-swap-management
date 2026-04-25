using BatterySwap.API.Data;
using BatterySwap.API.DTOs.Clients;
using BatterySwap.API.Helpers;
using BatterySwap.API.Services;
using BatterySwap.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BatterySwap.API.Controllers;

[ApiController]
[Route("api/clients")]
[Authorize(Roles = Roles.Admin + "," + Roles.Employee)]
public class ClientsController(AppDbContext dbContext, WalletService walletService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ClientListItemResponse>>> GetClients(CancellationToken cancellationToken)
    {
        var clients = await
            (from client in dbContext.Clients.AsNoTracking()
             join battery in dbContext.Batteries.AsNoTracking()
                 on client.CurrentBatteryId equals battery.Id into batteryGroup
             from battery in batteryGroup.DefaultIfEmpty()
             orderby client.Name
             select new ClientListItemResponse
             {
                 Id = client.Id,
                 Name = client.Name,
                 Phone = client.Phone,
                 VehicleType = client.VehicleType ?? "Not set",
                 VehicleNumber = client.VehicleNumber ?? "Not set",
                 CurrentBatteryCode = battery != null ? battery.BatteryCode : "Not assigned",
                 Balance = client.Balance,
                 Status = client.Status
             })
            .ToListAsync(cancellationToken);

        return Ok(clients);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClientDetailResponse>> GetClient(int id, CancellationToken cancellationToken)
    {
        var client = await GetClientDetailQuery()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (client is null)
        {
            return NotFound();
        }

        return Ok(client);
    }

    [HttpGet("search")]
    public async Task<ActionResult<ClientDetailResponse>> SearchByPhone([FromQuery] string phone, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return BadRequest(new { message = "Phone number is required." });
        }

        var client = await GetClientDetailQuery()
            .FirstOrDefaultAsync(x => x.Phone == phone.Trim(), cancellationToken);

        if (client is null)
        {
            return NotFound(new { message = "Client was not found." });
        }

        return Ok(client);
    }

    [HttpPost]
    public async Task<IActionResult> CreateClient([FromBody] CreateClientRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (await dbContext.Clients.AnyAsync(x => x.Phone == request.Phone, cancellationToken))
        {
            return BadRequest(new { message = "Phone number already exists." });
        }

        if (await dbContext.Clients.AnyAsync(x => x.Nid == request.Nid, cancellationToken))
        {
            return BadRequest(new { message = "NID already exists." });
        }

        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        var actorId = User.GetUserId() ?? 0;

        var client = new Models.Client
        {
            Name = request.Name.Trim(),
            Phone = request.Phone.Trim(),
            Nid = request.Nid.Trim(),
            Address = request.Address?.Trim(),
            VehicleType = request.VehicleType.Trim(),
            VehicleNumber = request.VehicleNumber.Trim(),
            Balance = BatterySwapDefaults.RegistrationBalance,
            Status = "Active",
            RegistrationDate = DateTime.UtcNow,
            CreatedByAdminId = role == Roles.Admin ? actorId : null,
            CreatedByEmployeeId = role == Roles.Employee ? actorId : null
        };

        dbContext.Clients.Add(client);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Created($"/api/clients/{client.Id}", new { client.Id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateClient(int id, [FromBody] UpdateClientRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var client = await dbContext.Clients.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (client is null)
        {
            return NotFound();
        }

        if (await dbContext.Clients.AnyAsync(x => x.Id != id && x.Phone == request.Phone, cancellationToken))
        {
            return BadRequest(new { message = "Phone number already exists." });
        }

        if (await dbContext.Clients.AnyAsync(x => x.Id != id && x.Nid == request.Nid, cancellationToken))
        {
            return BadRequest(new { message = "NID already exists." });
        }

        if (!string.Equals(request.Status, "Active", StringComparison.OrdinalIgnoreCase) && client.CurrentBatteryId.HasValue)
        {
            return BadRequest(new { message = "A client with an assigned battery cannot be marked inactive." });
        }

        client.Name = request.Name.Trim();
        client.Phone = request.Phone.Trim();
        client.Nid = request.Nid.Trim();
        client.Address = request.Address?.Trim();
        client.VehicleType = request.VehicleType.Trim();
        client.VehicleNumber = request.VehicleNumber.Trim();
        client.Status = request.Status.Trim();

        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeactivateClient(int id, CancellationToken cancellationToken)
    {
        var client = await dbContext.Clients.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (client is null)
        {
            return NotFound();
        }

        if (client.CurrentBatteryId.HasValue)
        {
            return BadRequest(new { message = "A client with an assigned battery cannot be deactivated." });
        }

        client.Status = "Inactive";
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/recharge")]
    public async Task<ActionResult<RechargeClientResponse>> RechargeClient(int id, [FromBody] RechargeClientRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? string.Empty;
        var actorId = User.GetUserId();
        if (!actorId.HasValue)
        {
            return Forbid();
        }

        var stationId = request.StationId;
        if (string.Equals(role, Roles.Employee, StringComparison.OrdinalIgnoreCase))
        {
            var employeeStationId = User.GetStationId();
            if (!employeeStationId.HasValue)
            {
                return Forbid();
            }

            stationId = employeeStationId.Value;
        }

        if (!stationId.HasValue)
        {
            return BadRequest(new { message = "Station is required for recharge." });
        }

        try
        {
            var response = await walletService.RechargeAsync(id, request.Amount, role, actorId.Value, stationId.Value, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private IQueryable<ClientDetailResponse> GetClientDetailQuery()
    {
        return from client in dbContext.Clients.AsNoTracking()
               join battery in dbContext.Batteries.AsNoTracking()
                   on client.CurrentBatteryId equals battery.Id into batteryGroup
               from battery in batteryGroup.DefaultIfEmpty()
               select new ClientDetailResponse
               {
                   Id = client.Id,
                   Name = client.Name,
                   Phone = client.Phone,
                   Nid = client.Nid,
                   Address = client.Address ?? string.Empty,
                   VehicleType = client.VehicleType ?? "Not set",
                   VehicleNumber = client.VehicleNumber ?? "Not set",
                   Balance = client.Balance,
                   Status = client.Status,
                   CurrentBatteryId = client.CurrentBatteryId,
                   CurrentBatteryCode = battery != null ? battery.BatteryCode : "Not assigned"
               };
    }
}
