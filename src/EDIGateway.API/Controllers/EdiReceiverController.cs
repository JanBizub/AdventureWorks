namespace EDI.API.Controllers;

using EDIGateway.API.Models;
using EDIGateway.API.Services;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class EdiReceiver(ILogger<EdiReceiver> logger, IEdiNotificationService notificationService) : ControllerBase
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
        logger.LogInformation("Received EDI message: {EdiMessage}", ediMessage);

        // Create EDI message object
        var ediMessageObj = new EdiMessage
        {
            Content = ediMessage ?? string.Empty,
            ReceivedAt = DateTime.UtcNow,
            Length = ediMessage?.Length ?? 0,
            MessageType = DetermineMessageType(ediMessage),
            Status = "Processing"
        };

        // Notify EDI Monitor in parallel
        _ = Task.Run(async () => await notificationService.NotifyMessageReceived(ediMessageObj));

        // TODO: Dependecy injection / Service
        var factory = new ConnectionFactory { HostName = "localhost" };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        // RabbitMQ receives utf-8 encoded byte arrays.
        var body = Encoding.UTF8.GetBytes(ediMessage ?? string.Empty);

        var properties = new BasicProperties
        {
            Persistent = true
        };

        // Declare EDI Exchange
        await channel.ExchangeDeclareAsync(
            exchange: "edi.inbound",
            type: ExchangeType.Direct,
            durable: true
            );

        // TODO: Determine routing key based on message type
        // edi.orders.incoming - For incoming order messages
        // edi.invoices.incoming - For incoming invoice messages
        // edi.shipments.incoming - For incoming shipment messages
        // edi.general - For general EDI processing

        // Declare Queue for incoming orders
        await channel.QueueDeclareAsync(
            queue: "edi.orders.incoming",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
            );

        // Bind the queue to the exchange with the routing key
        await channel.QueueBindAsync(
            queue: "edi.orders.incoming",
            exchange: "edi.inbound",
            routingKey: "edi.orders.incoming"
            );

        // Publish the message to the queue
        await channel.BasicPublishAsync(
            exchange: "edi.inbound",
            routingKey: "edi.orders.incoming",
            mandatory: true,
            basicProperties: properties,
            body: body
            );

        return Ok(new { status = "Received", length = ediMessage?.Length });
    }

    private static string DetermineMessageType(string? ediMessage)
    {
        if (string.IsNullOrEmpty(ediMessage))
            return "Unknown";

        // Simple logic to determine message type based on EDI content
        if (ediMessage.Contains("850"))
            return "Purchase Order";
        if (ediMessage.Contains("810"))
            return "Invoice";
        if (ediMessage.Contains("856"))
            return "Shipment Notice";
        
        return "General";
    }
}