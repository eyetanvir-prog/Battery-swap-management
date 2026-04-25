using System.ComponentModel.DataAnnotations;

namespace BatterySwap.MVC.Models;

public class CreateBatteryViewModel
{
    [Required]
    [Display(Name = "Battery Code")]
    public string BatteryCode { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Station")]
    public int StationId { get; set; }

    public List<StationOptionViewModel> Stations { get; set; } = [];
    public string? ErrorMessage { get; set; }
}
