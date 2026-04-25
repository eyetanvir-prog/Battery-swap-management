namespace BatterySwap.API.DTOs.Recharges;

public class RechargeHistoryItemResponse
{
    public int Id { get; init; }
    public int ClientId { get; init; }
    public string ClientName { get; init; } = string.Empty;
    public string ClientPhone { get; init; } = string.Empty;
    public string StationName { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateTime RechargeTime { get; init; }
    public string AddedBy { get; init; } = string.Empty;
}
