namespace BatterySwap.MVC.Models;

public class ApiClientListItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty;
    public string VehicleNumber { get; set; } = string.Empty;
    public string CurrentBatteryCode { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Status { get; set; } = string.Empty;
}
