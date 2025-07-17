namespace EDIMonitor.API.Controllers;
using EDIMonitor.API.Hubs;
using EDIMonitor.API.Models;
using EDIMonitor.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

[ApiController]
[Route("api/[controller]")]
public class EdiMonitor(ILogger<EdiMonitor> logger, IEdiMessageService messageService, IHubContext<EdiMonitorHub> hubContext) : ControllerBase
{
    [HttpGet("echo")]
    public IActionResult Echo()
    {
        return Ok("EdiMonitor::Echo");
    }

    [HttpPost("message-received")]
    public async Task<IActionResult> MessageReceived([FromBody] EdiMessage message)
    {
        logger.LogInformation("Received notification of EDI message: {MessageType}, Length: {Length}", 
            message.MessageType, message.Length);

        // Store the message
        await messageService.AddMessage(message);

        // Notify all connected clients via SignalR
        await hubContext.Clients.All.SendAsync("MessageReceived", new EdiMessageSummary
        {
            Id = message.Id,
            ReceivedAt = message.ReceivedAt,
            MessageType = message.MessageType,
            Length = message.Length,
            Status = message.Status,
            SourceIdentifier = message.SourceIdentifier
        });

        return Ok(new { status = "Processed", messageId = message.Id });
    }

    [HttpGet("messages")]
    public async Task<IActionResult> GetMessages([FromQuery] int count = 50)
    {
        var messages = await messageService.GetRecentMessages(count);
        return Ok(messages);
    }

    [HttpGet("messages/{id}")]
    public async Task<IActionResult> GetMessage(int id)
    {
        var message = await messageService.GetMessage(id);
        if (message == null)
        {
            return NotFound();
        }
        return Ok(message);
    }
}