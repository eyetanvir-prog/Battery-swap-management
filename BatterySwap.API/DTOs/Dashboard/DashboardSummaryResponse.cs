namespace BatterySwap.API.DTOs.Dashboard;

public class DashboardSummaryResponse
{
    public IReadOnlyList<DashboardStatItemResponse> StatCards { get; init; } = [];
    public IReadOnlyList<DashboardStatItemResponse> Highlights { get; init; } = [];
    public IReadOnlyList<DashboardActivityItemResponse> RecentActivities { get; init; } = [];
    public IReadOnlyList<DashboardStationStockResponse> StationStocks { get; init; } = [];
}

public class DashboardStatItemResponse
{
    public string Title { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    public string AccentClass { get; init; } = string.Empty;
    public string IconClass { get; init; } = string.Empty;
}

public class DashboardActivityItemResponse
{
    public string Title { get; init; } = string.Empty;
    public string Detail { get; init; } = string.Empty;
    public DateTime TimestampUtc { get; init; }
    public string BadgeClass { get; init; } = string.Empty;
    public string BadgeText { get; init; } = string.Empty;
}

public class DashboardStationStockResponse
{
    public string StationName { get; init; } = string.Empty;
    public string StationCode { get; init; } = string.Empty;
    public int AvailableBatteries { get; init; }
    public int TotalBatteries { get; init; }
    public string EmployeeName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
}
