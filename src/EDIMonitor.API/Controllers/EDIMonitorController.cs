namespace EDIMonitor.API.Controllers;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EdiMonitor(ILogger<EdiMonitor> logger) : ControllerBase
{
    [HttpGet("echo")]
    public IActionResult Echo()
    {
        return Ok("EdiMonitor::Echo");
    }

}