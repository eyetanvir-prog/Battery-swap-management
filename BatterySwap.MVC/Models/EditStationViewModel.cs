using System.ComponentModel.DataAnnotations;

namespace BatterySwap.MVC.Models;

public class EditStationViewModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Station Name")]
    public string StationName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Station Code")]
    public string StationCode { get; set; } = string.Empty;

    public string? Address { get; set; }

    [Range(-90, 90)]
    public decimal? Latitude { get; set; }

    [Range(-180, 180)]
    public decimal? Longitude { get; set; }

    [Required]
    public string Status { get; set; } = "Open";

    public string? ErrorMessage { get; set; }
}
