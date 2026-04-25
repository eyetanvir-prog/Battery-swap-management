namespace BatterySwap.MVC.Models;

public class ApiBatteryDetail
{
    public int Id { get; set; }
    public string BatteryCode { get; set; } = string.Empty;
    public int StationId { get; set; }
    public string StationName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? CurrentClientId { get; set; }
    public string CurrentClientName { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}
