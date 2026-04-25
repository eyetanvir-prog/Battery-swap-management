namespace BatterySwap.MVC.Options;

public class ApiSettings
{
    public const string SectionName = "ApiSettings";

    public string BaseUrl { get; set; } = "http://localhost:8081";
}
