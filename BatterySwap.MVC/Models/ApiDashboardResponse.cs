namespace BatterySwap.MVC.Models;

public class ApiDashboardResponse
{
    public List<ApiDashboardStatItem> StatCards { get; set; } = [];
    public List<ApiDashboardStatItem> Highlights { get; set; } = [];
    public List<ApiDashboardActivityItem> RecentActivities { get; set; } = [];
    public List<ApiDashboardStationStockItem> StationStocks { get; set; } = [];
}

public class ApiDashboardStatItem
{
    public string Title { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string AccentClass { get; set; } = string.Empty;
    public string IconClass { get; set; } = string.Empty;
}

public class ApiDashboardActivityItem
{
    public string Title { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public DateTime TimestampUtc { get; set; }
    public string BadgeClass { get; set; } = string.Empty;
    public string BadgeText { get; set; } = string.Empty;
}

public class ApiDashboardStationStockItem
{
    public string StationName { get; set; } = string.Empty;
    public string StationCode { get; set; } = string.Empty;
    public int AvailableBatteries { get; set; }
    public int TotalBatteries { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
