namespace BatterySwap.API.DTOs.Stations;

public class StationLookupResponse
{
    public int Id { get; init; }
    public string StationName { get; init; } = string.Empty;
    public string StationCode { get; init; } = string.Empty;
}
