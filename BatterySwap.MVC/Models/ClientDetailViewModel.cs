namespace BatterySwap.MVC.Models;

public class ClientDetailViewModel
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string NID { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string VehicleType { get; init; } = string.Empty;
    public string VehicleNumber { get; init; } = string.Empty;
    public decimal BalanceValue { get; init; }
    public string BalanceDisplay { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public int? CurrentBatteryId { get; init; }
    public string CurrentBatteryCode { get; init; } = string.Empty;
}
