namespace BatterySwap.MVC.Models;

public class ApiEmployeeDetail
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Nid { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int? StationId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateOnly? JoiningDate { get; set; }
}
