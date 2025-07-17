using Microsoft.AspNetCore.Http.HttpResults;

namespace EDI.API.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EdiReceiver(ILogger<EdiReceiver> logger) : ControllerBase
{
    [HttpGet("echo")]
    public IActionResult Echo()
    {
        return Ok("Hello");
    }

    [HttpPost("receive")]
    [Consumes("text/plain")]
    public async Task<IActionResult> Receive([FromBody] string ediMessage)
    {
        // Log the received message (optional)
        logger.LogInformation("Received EDI message: {EdiMessage}", ediMessage);

        // You can add processing logic here

        return Ok(new { status = "Received", length = ediMessage?.Length });
    }
}