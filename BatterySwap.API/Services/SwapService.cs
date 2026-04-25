using BatterySwap.API.Data;
using BatterySwap.API.DTOs.Swaps;
using BatterySwap.API.Models;
using BatterySwap.Shared.Constants;
using Microsoft.EntityFrameworkCore;

namespace BatterySwap.API.Services;

public class SwapService(AppDbContext dbContext)
{
    public async Task<ProcessSwapResponse> ProcessSwapAsync(
        int clientId,
        int assignedBatteryId,
        int stationId,
        int employeeId,
        CancellationToken cancellationToken = default)
    {
        var client = await dbContext.Clients.FirstOrDefaultAsync(x => x.Id == clientId, cancellationToken)
            ?? throw new InvalidOperationException("Client was not found.");

        if (!string.Equals(client.Status, "Active", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only active clients can perform swaps.");
        }

        if (client.CurrentBatteryId is null)
        {
            throw new InvalidOperationException("Client does not currently hold a battery to return.");
        }

        if (client.Balance < BatterySwapDefaults.SwapCost)
        {
            throw new InvalidOperationException("Client balance is below the swap cost.");
        }

        var returnedBattery = await dbContext.Batteries.FirstOrDefaultAsync(x => x.Id == client.CurrentBatteryId.Value, cancellationToken)
            ?? throw new InvalidOperationException("Returned battery could not be found.");

        var assignedBattery = await dbContext.Batteries.FirstOrDefaultAsync(x => x.Id == assignedBatteryId, cancellationToken)
            ?? throw new InvalidOperationException("Selected battery could not be found.");

        if (returnedBattery.Id == assignedBattery.Id)
        {
            throw new InvalidOperationException("Returned battery and assigned battery must be different.");
        }

        if (!string.Equals(assignedBattery.Status, "Available", StringComparison.OrdinalIgnoreCase) || assignedBattery.StationId != stationId)
        {
            throw new InvalidOperationException("Selected battery is not available at this station.");
        }

        var employeeBelongsToStation = await dbContext.Employees.AnyAsync(
            x => x.Id == employeeId && x.StationId == stationId && x.Status == "Active",
            cancellationToken);

        if (!employeeBelongsToStation)
        {
            throw new InvalidOperationException("Employee is not assigned to this station.");
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        client.Balance -= BatterySwapDefaults.SwapCost;
        client.CurrentBatteryId = assignedBattery.Id;

        returnedBattery.Status = "Available";
        returnedBattery.StationId = stationId;
        returnedBattery.CurrentClientId = null;
        returnedBattery.LastUpdated = DateTime.UtcNow;

        assignedBattery.Status = "WithClient";
        assignedBattery.CurrentClientId = client.Id;
        assignedBattery.LastUpdated = DateTime.UtcNow;

        dbContext.Swaps.Add(new Swap
        {
            ClientId = client.Id,
            StationId = stationId,
            ReturnedBatteryId = returnedBattery.Id,
            AssignedBatteryId = assignedBattery.Id,
            SwapCost = BatterySwapDefaults.SwapCost,
            SwapTime = DateTime.UtcNow,
            ProcessedByEmployeeId = employeeId
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new ProcessSwapResponse
        {
            ClientId = client.Id,
            StationId = stationId,
            SwapCost = BatterySwapDefaults.SwapCost,
            NewBalance = client.Balance,
            ReturnedBatteryCode = returnedBattery.BatteryCode,
            AssignedBatteryCode = assignedBattery.BatteryCode
        };
    }
}
