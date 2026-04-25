namespace BatterySwap.API.DTOs.Swaps;

public class SwapHistoryItemResponse
{
    public int Id { get; init; }
    public int ClientId { get; init; }
    public string ClientName { get; init; } = string.Empty;
    public string ClientPhone { get; init; } = string.Empty;
    public string StationName { get; init; } = string.Empty;
    public string ReturnedBatteryCode { get; init; } = string.Empty;
    public string AssignedBatteryCode { get; init; } = string.Empty;
    public decimal SwapCost { get; init; }
    public DateTime SwapTime { get; init; }
    public string ProcessedByEmployee { get; init; } = string.Empty;
}
