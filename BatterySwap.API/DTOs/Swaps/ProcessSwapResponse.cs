namespace BatterySwap.API.DTOs.Swaps;

public class ProcessSwapResponse
{
    public int ClientId { get; init; }
    public int StationId { get; init; }
    public decimal SwapCost { get; init; }
    public decimal NewBalance { get; init; }
    public string ReturnedBatteryCode { get; init; } = string.Empty;
    public string AssignedBatteryCode { get; init; } = string.Empty;
}
