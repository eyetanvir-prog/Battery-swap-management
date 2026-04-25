namespace BatterySwap.API.DTOs.Batteries;

public class BatteryDetailResponse
{
    public int Id { get; init; }
    public string BatteryCode { get; init; } = string.Empty;
    public int StationId { get; init; }
    public string StationName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public int? CurrentClientId { get; init; }
    public string CurrentClientName { get; init; } = string.Empty;
    public DateTime LastUpdated { get; init; }
}
