namespace BatterySwap.MVC.Models;

public class EmployeeListViewModel
{
    public required IReadOnlyList<EmployeeSummaryViewModel> Employees { get; init; }
}

public class EmployeeSummaryViewModel
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Username { get; init; }
    public required string Phone { get; init; }
    public required string NID { get; init; }
    public required string StationName { get; init; }
    public required string Status { get; init; }
    public required string JoiningDate { get; init; }
}
