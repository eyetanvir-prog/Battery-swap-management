using System.ComponentModel.DataAnnotations;

namespace BatterySwap.API.DTOs.Stations;

public class UpdateStationRequest
{
    [Required]
    public string StationName { get; set; } = string.Empty;

    [Required]
    public string StationCode { get; set; } = string.Empty;

    public string? Address { get; set; }

    [Range(-90, 90)]
    public decimal? Latitude { get; set; }

    [Range(-180, 180)]
    public decimal? Longitude { get; set; }

    [Required]
    public string Status { get; set; } = "Open";
}
