namespace EDI.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;

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
        logger.LogInformation("Received EDI message: {EdiMessage}", ediMessage);

        // TODO: Dependecy injection / Service
        var factory = new ConnectionFactory { HostName = "localhost" };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        // RabbitMQ receives utf-8 encoded byte arrays.
        var body = Encoding.UTF8.GetBytes(ediMessage);

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
}