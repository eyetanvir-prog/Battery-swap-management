namespace BatterySwap.MVC.Models;

public class SwapHistoryListViewModel
{
    public required IReadOnlyList<SwapHistoryItemViewModel> Items { get; init; }
}

public class SwapHistoryItemViewModel
{
    public int Id { get; init; }
    public int ClientId { get; init; }
    public string ClientName { get; init; } = string.Empty;
    public string ClientPhone { get; init; } = string.Empty;
    public string StationName { get; init; } = string.Empty;
    public string ReturnedBatteryCode { get; init; } = string.Empty;
    public string AssignedBatteryCode { get; init; } = string.Empty;
    public string SwapCost { get; init; } = string.Empty;
    public string SwapTime { get; init; } = string.Empty;
    public string ProcessedByEmployee { get; init; } = string.Empty;
}
