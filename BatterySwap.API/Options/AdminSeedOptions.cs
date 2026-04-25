using BatterySwap.Shared.Constants;

namespace BatterySwap.API.Options;

public class AdminSeedOptions
{
    public const string SectionName = "AdminSeed";

    public string Name { get; set; } = BatterySwapDefaults.DefaultAdminName;
    public string Phone { get; set; } = BatterySwapDefaults.DefaultAdminPhone;
    public string Email { get; set; } = BatterySwapDefaults.DefaultAdminEmail;
    public string Password { get; set; } = BatterySwapDefaults.DefaultAdminPassword;
}
