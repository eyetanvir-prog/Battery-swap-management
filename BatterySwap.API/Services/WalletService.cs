using BatterySwap.API.Data;
using BatterySwap.API.DTOs.Clients;
using BatterySwap.API.Models;
using BatterySwap.Shared.Constants;
using Microsoft.EntityFrameworkCore;

namespace BatterySwap.API.Services;

public class WalletService(AppDbContext dbContext)
{
    public async Task<RechargeClientResponse> RechargeAsync(
        int clientId,
        decimal amount,
        string role,
        int actorId,
        int stationId,
        CancellationToken cancellationToken = default)
    {
        if (amount <= 0)
        {
            throw new InvalidOperationException("Recharge amount must be greater than zero.");
        }

        var client = await dbContext.Clients.FirstOrDefaultAsync(x => x.Id == clientId, cancellationToken)
            ?? throw new InvalidOperationException("Client was not found.");

        if (!string.Equals(client.Status, "Active", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only active clients can be recharged.");
        }

        var stationExists = await dbContext.Stations.AnyAsync(x => x.Id == stationId, cancellationToken);
        if (!stationExists)
        {
            throw new InvalidOperationException("Station was not found.");
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        client.Balance += amount;

        dbContext.BalanceRecharges.Add(new BalanceRecharge
        {
            ClientId = client.Id,
            StationId = stationId,
            Amount = amount,
            AddedByAdminId = role == Roles.Admin ? actorId : null,
            AddedByEmployeeId = role == Roles.Employee ? actorId : null,
            RechargeTime = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new RechargeClientResponse
        {
            ClientId = client.Id,
            RechargeAmount = amount,
            NewBalance = client.Balance,
            StationId = stationId
        };
    }
}
