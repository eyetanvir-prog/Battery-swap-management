using Microsoft.AspNetCore.Mvc;

namespace BatterySwap.API.Controllers;

[ApiController]
[Route("api/system")]
public class SystemController : ControllerBase
{
    [HttpGet("info")]
    public IActionResult GetInfo()
    {
        return Ok(new
        {
            application = "BatterySwap.API",
            status = "ready",
            timestampUtc = DateTime.UtcNow
        });
    }
}
