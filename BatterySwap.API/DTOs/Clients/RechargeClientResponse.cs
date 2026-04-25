namespace BatterySwap.API.DTOs.Clients;

public class RechargeClientResponse
{
    public int ClientId { get; init; }
    public decimal RechargeAmount { get; init; }
    public decimal NewBalance { get; init; }
    public int StationId { get; init; }
}
