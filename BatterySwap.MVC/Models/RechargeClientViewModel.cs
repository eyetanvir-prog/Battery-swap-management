using System.ComponentModel.DataAnnotations;

namespace BatterySwap.MVC.Models;

public class RechargeClientViewModel
{
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string CurrentBalance { get; set; } = string.Empty;
    public bool RequiresStationSelection { get; set; }
    public List<StationOptionViewModel> Stations { get; set; } = [];

    [Range(1, 1000000)]
    public decimal Amount { get; set; }

    [Display(Name = "Station")]
    public int? StationId { get; set; }

    public string? ErrorMessage { get; set; }
}
