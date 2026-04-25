using System.ComponentModel.DataAnnotations;

namespace BatterySwap.API.DTOs.Batteries;

public class CreateBatteryRequest
{
    [Required]
    public string BatteryCode { get; set; } = string.Empty;

    [Required]
    public int StationId { get; set; }
}
