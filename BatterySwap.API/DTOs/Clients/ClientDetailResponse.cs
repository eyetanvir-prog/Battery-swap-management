namespace BatterySwap.API.DTOs.Clients;

public class ClientDetailResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Nid { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string VehicleType { get; init; } = string.Empty;
    public string VehicleNumber { get; init; } = string.Empty;
    public decimal Balance { get; init; }
    public string Status { get; init; } = string.Empty;
    public int? CurrentBatteryId { get; init; }
    public string CurrentBatteryCode { get; init; } = string.Empty;
}
