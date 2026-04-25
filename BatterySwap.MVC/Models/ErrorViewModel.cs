namespace BatterySwap.MVC.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public int? StatusCode { get; set; }
    public string PageTitle { get; set; } = "Request Error";
    public string Headline { get; set; } = "Something went wrong.";
    public string Description { get; set; } = "The request could not be completed.";
    public string? ActionLabel { get; set; }
    public string? ActionUrl { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
