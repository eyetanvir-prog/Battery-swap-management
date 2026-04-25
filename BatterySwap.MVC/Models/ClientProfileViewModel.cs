namespace BatterySwap.MVC.Models;

public class ClientProfileViewModel
{
    public required ClientDetailViewModel Client { get; init; }
    public required IReadOnlyList<SwapHistoryItemViewModel> SwapHistory { get; init; }
}
