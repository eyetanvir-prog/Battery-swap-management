using System.ComponentModel.DataAnnotations;

namespace BatterySwap.MVC.Models;

public class ProcessSwapViewModel
{
    [Display(Name = "Client Phone")]
    public string SearchPhone { get; set; } = string.Empty;

    public ClientDetailViewModel? Client { get; set; }
    public List<BatteryOptionViewModel> AvailableBatteries { get; set; } = [];

    [Required]
    public int ClientId { get; set; }

    [Required]
    [Display(Name = "Assign New Battery")]
    public int AssignedBatteryId { get; set; }

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
}
