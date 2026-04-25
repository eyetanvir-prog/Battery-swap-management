namespace BatterySwap.MVC.Models;

public class DashboardViewModel
{
    public required IReadOnlyList<StatCardViewModel> StatCards { get; init; }
    public required IReadOnlyList<StatCardViewModel> Highlights { get; init; }
    public required IReadOnlyList<ActivityItemViewModel> RecentActivities { get; init; }
    public required IReadOnlyList<StationStockViewModel> StationStocks { get; init; }
}

public class StatCardViewModel
{
    public required string Title { get; init; }
    public required string Value { get; init; }
    public required string AccentClass { get; init; }
    public required string IconClass { get; init; }
    public required string Subtitle { get; init; }
}

public class ActivityItemViewModel
{
    public required string Title { get; init; }
    public required string Detail { get; init; }
    public required string RelativeTime { get; init; }
    public required string BadgeClass { get; init; }
    public required string BadgeText { get; init; }
}

public class StationStockViewModel
{
    public required string StationName { get; init; }
    public required string StationCode { get; init; }
    public int AvailableBatteries { get; init; }
    public int TotalBatteries { get; init; }
    public required string EmployeeName { get; init; }
    public required string Status { get; init; }
}
