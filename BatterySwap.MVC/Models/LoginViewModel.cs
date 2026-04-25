using System.ComponentModel.DataAnnotations;

namespace BatterySwap.MVC.Models;

public class LoginViewModel
{
    [Required]
    [Display(Name = "Email or Username")]
    public string UsernameOrEmail { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
    public string? InfoMessage { get; set; }
    public string? ReturnUrl { get; set; }
}
