using System.ComponentModel.DataAnnotations;

namespace BatterySwap.MVC.Models;

public class EditBatteryViewModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Battery Code")]
    public string BatteryCode { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Station")]
    public int StationId { get; set; }

    public string Status { get; set; } = string.Empty;
    public string CurrentClientName { get; set; } = string.Empty;
    public bool IsAssignedToClient { get; set; }
    public string LastUpdated { get; set; } = string.Empty;
    public List<StationOptionViewModel> Stations { get; set; } = [];
    public string? ErrorMessage { get; set; }
}
