namespace BatterySwap.API.DTOs.Employees;

public class EmployeeListItemResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Nid { get; init; } = string.Empty;
    public string StationName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateOnly? JoiningDate { get; init; }
}
