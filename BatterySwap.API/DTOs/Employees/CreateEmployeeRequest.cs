using System.ComponentModel.DataAnnotations;

namespace BatterySwap.API.DTOs.Employees;

public class CreateEmployeeRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string Nid { get; set; } = string.Empty;

    public string? Address { get; set; }

    [Required]
    public int StationId { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public DateOnly? JoiningDate { get; set; }
}
