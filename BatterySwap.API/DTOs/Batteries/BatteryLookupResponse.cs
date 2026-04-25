namespace BatterySwap.API.DTOs.Batteries;

public class BatteryLookupResponse
{
    public int Id { get; init; }
    public string BatteryCode { get; init; } = string.Empty;
}
