using System.ComponentModel.DataAnnotations;

namespace BatterySwap.API.DTOs.Clients;

public class CreateClientRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string Nid { get; set; } = string.Empty;

    public string? Address { get; set; }

    [Required]
    public string VehicleType { get; set; } = string.Empty;

    [Required]
    public string VehicleNumber { get; set; } = string.Empty;
}
