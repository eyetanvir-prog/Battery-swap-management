using System.ComponentModel.DataAnnotations;

namespace BatterySwap.MVC.Models;

public class CreateEmployeeViewModel
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [Display(Name = "NID")]
    public string NID { get; set; } = string.Empty;

    public string? Address { get; set; }

    [Required]
    [Display(Name = "Station")]
    public int StationId { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Joining Date")]
    public DateTime? JoiningDate { get; set; } = DateTime.Today;

    public List<StationOptionViewModel> Stations { get; set; } = [];
    public string? ErrorMessage { get; set; }
}
