namespace BatterySwap.MVC.Models;

public class ReportSummaryViewModel
{
    public string ScopeLabel { get; init; } = string.Empty;
    public int StationCount { get; init; }
    public int TotalSwaps { get; init; }
    public decimal TotalSwapRevenue { get; init; }
    public int TotalRecharges { get; init; }
    public decimal TotalRechargeAmount { get; init; }
    public int LowAvailabilityStations { get; init; }
    public required IReadOnlyList<StationStockViewModel> StationStocks { get; init; }
    public required IReadOnlyList<SwapHistoryItemViewModel> RecentSwaps { get; init; }
    public required IReadOnlyList<RechargeHistoryItemViewModel> RecentRecharges { get; init; }
}
