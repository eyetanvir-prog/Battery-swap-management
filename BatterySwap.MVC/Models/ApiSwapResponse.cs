namespace BatterySwap.MVC.Models;

public class ApiSwapResponse
{
    public int ClientId { get; set; }
    public int StationId { get; set; }
    public decimal SwapCost { get; set; }
    public decimal NewBalance { get; set; }
    public string ReturnedBatteryCode { get; set; } = string.Empty;
    public string AssignedBatteryCode { get; set; } = string.Empty;
}
