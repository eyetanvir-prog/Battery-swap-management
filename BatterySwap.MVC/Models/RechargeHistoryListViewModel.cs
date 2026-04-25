namespace BatterySwap.MVC.Models;

public class RechargeHistoryListViewModel
{
    public required IReadOnlyList<RechargeHistoryItemViewModel> Items { get; init; }
}

public class RechargeHistoryItemViewModel
{
    public int Id { get; init; }
    public int ClientId { get; init; }
    public string ClientName { get; init; } = string.Empty;
    public string ClientPhone { get; init; } = string.Empty;
    public string StationName { get; init; } = string.Empty;
    public string Amount { get; init; } = string.Empty;
    public string RechargeTime { get; init; } = string.Empty;
    public string AddedBy { get; init; } = string.Empty;
}
