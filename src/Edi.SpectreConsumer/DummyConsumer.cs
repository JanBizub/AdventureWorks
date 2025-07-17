using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

class DummyConsumer(IChannel channel, string queueName, ILogger? logger = null) : IAsyncDisposable
{
    private readonly AsyncEventingBasicConsumer consumer = new(channel);
    public readonly string ConsumerTag = Guid.NewGuid().ToString();
    private readonly CancellationTokenSource _cts = new();

    private async Task HandleReceivedAsync(object sender, BasicDeliverEventArgs ea)
    {
        try
        {
            byte[] body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            logger?.LogInformation($"Consumer: [{ConsumerTag}] received {message}");

            await Task.Delay(5000, _cts.Token);

            await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            logger?.LogInformation($"Consumer: [{ConsumerTag}] Done");
        }
        catch (OperationCanceledException)
        {
            logger?.LogInformation($"Consumer: [{ConsumerTag}] Cancelled");
        }
        catch (Exception ex)
        {
            logger?.LogError($"Consumer: [{ConsumerTag}] Error: {ex.Message}");
            // Optionally NACK the message if processing failed
            await channel.BasicNackAsync(ea.DeliveryTag, false, true);
        }
    }

    public async Task SubscribeToQueueAsync()
    {
        consumer.ReceivedAsync += HandleReceivedAsync;

        await channel.BasicConsumeAsync(
            queueName,
            autoAck: false,
            consumer: consumer,
            consumerTag: ConsumerTag.ToString()
        );

        logger?.LogInformation($"Consumer: [{ConsumerTag}] subscribed to {queueName}");
    }


    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();

        try
        {
            await channel.BasicCancelAsync(ConsumerTag.ToString());
        }
        catch (Exception ex)
        {
            logger?.LogInformation($"Error cancelling consumer: {ex.Message}");
        }

        consumer.ReceivedAsync -= HandleReceivedAsync;

        _cts.Dispose();
    }
}