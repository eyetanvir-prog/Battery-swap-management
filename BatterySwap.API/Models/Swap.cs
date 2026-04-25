using BatterySwap.Shared.Constants;

namespace BatterySwap.API.Models;

public class Swap
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int StationId { get; set; }
    public int ReturnedBatteryId { get; set; }
    public int AssignedBatteryId { get; set; }
    public decimal SwapCost { get; set; } = BatterySwapDefaults.SwapCost;
    public DateTime SwapTime { get; set; } = DateTime.UtcNow;
    public int ProcessedByEmployeeId { get; set; }
}
