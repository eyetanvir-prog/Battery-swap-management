namespace BatterySwap.API.Models;

public class BalanceRecharge
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int StationId { get; set; }
    public decimal Amount { get; set; }
    public int? AddedByAdminId { get; set; }
    public int? AddedByEmployeeId { get; set; }
    public DateTime RechargeTime { get; set; } = DateTime.UtcNow;
}
