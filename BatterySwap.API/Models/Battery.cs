namespace BatterySwap.API.Models;

public class Battery
{
    public int Id { get; set; }
    public string BatteryCode { get; set; } = string.Empty;
    public int StationId { get; set; }
    public string Status { get; set; } = "Available";
    public int? CurrentClientId { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
