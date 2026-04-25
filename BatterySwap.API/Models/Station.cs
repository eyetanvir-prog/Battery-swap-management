namespace BatterySwap.API.Models;

public class Station
{
    public int Id { get; set; }
    public string StationName { get; set; } = string.Empty;
    public string StationCode { get; set; } = string.Empty;
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int? EmployeeId { get; set; }
    public string Status { get; set; } = "Open";
}
