namespace BatterySwap.MVC.Models;

public class ClientListViewModel
{
    public required IReadOnlyList<ClientSummaryViewModel> Clients { get; init; }
}

public class ClientSummaryViewModel
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Phone { get; init; }
    public required string VehicleType { get; init; }
    public required string VehicleNumber { get; init; }
    public required string CurrentBatteryCode { get; init; }
    public required string Balance { get; init; }
    public required string Status { get; init; }
    public required string DisplayStatus { get; init; }
    public bool IsInactive { get; init; }
}
