using System.ComponentModel.DataAnnotations;

namespace BatterySwap.API.DTOs.Batteries;

public class UpdateBatteryRequest
{
    [Required]
    public string BatteryCode { get; set; } = string.Empty;

    [Required]
    public int StationId { get; set; }
}
