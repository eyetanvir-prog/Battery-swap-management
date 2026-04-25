using System.ComponentModel.DataAnnotations;

namespace BatterySwap.API.DTOs.Swaps;

public class ProcessSwapRequest
{
    [Required]
    public int ClientId { get; set; }

    [Required]
    public int AssignedBatteryId { get; set; }
}
