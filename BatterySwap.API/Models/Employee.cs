namespace BatterySwap.API.Models;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Nid { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? PhotoPath { get; set; }
    public int? StationId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public DateOnly? JoiningDate { get; set; }
}
