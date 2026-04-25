namespace BatterySwap.MVC.Models;

public class ApiRechargeHistoryItem
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string ClientPhone { get; set; } = string.Empty;
    public string StationName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime RechargeTime { get; set; }
    public string AddedBy { get; set; } = string.Empty;
}
