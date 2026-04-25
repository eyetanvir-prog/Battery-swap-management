namespace BatterySwap.API.Models;

public class Client
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Nid { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? PhotoPath { get; set; }
    public string? VehicleType { get; set; }
    public string? VehicleNumber { get; set; }
    public decimal Balance { get; set; }
    public int? CurrentBatteryId { get; set; }
    public int? CreatedByAdminId { get; set; }
    public int? CreatedByEmployeeId { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Active";
}
