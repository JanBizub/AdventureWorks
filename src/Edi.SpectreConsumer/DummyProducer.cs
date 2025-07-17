using System.Text;
using RabbitMQ.Client;

public class DummyProducer
{
    private readonly IChannel channel;

    public DummyProducer(IChannel channel)
    {
        this.channel = channel;
    }

    public async Task SpawnMessageAsync(string message = "Hello", string queue = "task_queue")
    {
        var properties = new BasicProperties { Persistent = true };
        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queue,
            mandatory: true,
            basicProperties: properties,
            body: body
        );
    }
}