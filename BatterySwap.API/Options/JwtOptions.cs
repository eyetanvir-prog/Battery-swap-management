namespace BatterySwap.API.Options;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = "super-secret-jwt-key-minimum-32-chars";
    public string Issuer { get; set; } = "BatterySwapAPI";
    public string Audience { get; set; } = "BatterySwapClients";
    public int ExpiryMinutes { get; set; } = 60;
}
