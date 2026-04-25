using System.ComponentModel.DataAnnotations;

namespace BatterySwap.API.DTOs.Clients;

public class RechargeClientRequest
{
    [Range(1, 1000000)]
    public decimal Amount { get; set; }

    public int? StationId { get; set; }
}
