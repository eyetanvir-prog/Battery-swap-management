namespace BatterySwap.MVC.Models;

public class ApiSwapHistoryItem
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string ClientPhone { get; set; } = string.Empty;
    public string StationName { get; set; } = string.Empty;
    public string ReturnedBatteryCode { get; set; } = string.Empty;
    public string AssignedBatteryCode { get; set; } = string.Empty;
    public decimal SwapCost { get; set; }
    public DateTime SwapTime { get; set; }
    public string ProcessedByEmployee { get; set; } = string.Empty;
}
