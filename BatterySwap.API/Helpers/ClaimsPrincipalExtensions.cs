using System.Security.Claims;

namespace BatterySwap.API.Helpers;

public static class ClaimsPrincipalExtensions
{
    public static int? GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue("sub");

        return int.TryParse(value, out var id) ? id : null;
    }

    public static int? GetStationId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue("station_id");
        return int.TryParse(value, out var id) ? id : null;
    }
}
