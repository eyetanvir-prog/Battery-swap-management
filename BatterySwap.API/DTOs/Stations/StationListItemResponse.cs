namespace BatterySwap.API.DTOs.Stations;

public class StationListItemResponse
{
    public int Id { get; init; }
    public string StationName { get; init; } = string.Empty;
    public string StationCode { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }
    public int AvailableBatteries { get; init; }
    public int TotalBatteries { get; init; }
    public string EmployeeName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
}
