using System.ComponentModel.DataAnnotations;

namespace BatterySwap.MVC.Models;

public class EditClientViewModel
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [Display(Name = "NID")]
    public string NID { get; set; } = string.Empty;

    public string? Address { get; set; }

    [Required]
    [Display(Name = "Vehicle Type")]
    public string VehicleType { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Vehicle Number")]
    public string VehicleNumber { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = "Active";

    public string CurrentBatteryCode { get; set; } = "Not assigned";
    public string BalanceDisplay { get; set; } = "0 Tk";
    public string? ErrorMessage { get; set; }
}
