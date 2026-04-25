namespace BatterySwap.MVC.Models;

public class BatteryListViewModel
{
    public required IReadOnlyList<BatterySummaryViewModel> Batteries { get; init; }
}

public class BatterySummaryViewModel
{
    public int Id { get; init; }
    public string BatteryCode { get; init; } = string.Empty;
    public int StationId { get; init; }
    public string StationName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public int? CurrentClientId { get; init; }
    public string CurrentClientName { get; init; } = string.Empty;
    public string LastUpdated { get; init; } = string.Empty;
}
