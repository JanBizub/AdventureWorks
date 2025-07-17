// See https://aka.ms/new-console-template for more information

using Spectre.Console;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Utils;

AnsiConsole.MarkupLine("[bold yellow] Welcome to Message Broker Prototyping! [/]");
AnsiConsole.MarkupLine("Connecting to RabbitMQ instance.");

var factory = new ConnectionFactory { HostName = "localhost" };

try
{
    await using var connection = await factory.CreateConnectionAsync();
    await using var tasksChannel = await connection.CreateChannelAsync();

    AnsiConsole.MarkupLine("Connection Established.");

    await tasksChannel.QueueDeclareAsync(
        queue: "edi.orders.incoming",
        durable: true,
        exclusive: false,
        autoDelete: false,
        arguments: null
    );

    var dummyProducer = new DummyProducer(tasksChannel);

    // keep a track of refernces to the running consumers
    IList<DummyConsumer> runningDummyConsumers = [];

    var quitProgramCancellationSource = new CancellationTokenSource();
    _ = Task.Run(async () =>
    {
        while (true)
        {
            var key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.Escape:
                    quitProgramCancellationSource.Cancel();
                    break;

                case ConsoleKey.D1:
                    await dummyProducer.SpawnMessageAsync();
                    AnsiConsole.MarkupLine("2 - Spawned 1 message!");
                    break;

                case ConsoleKey.D2:
                    // TODO: implement a logger and add it as param
                    var newConsumer = new DummyConsumer(tasksChannel,"edi.orders.incoming"); 
                    await newConsumer.SubscribeToQueueAsync();
                    runningDummyConsumers.Add(newConsumer);
                    break;

                case ConsoleKey.D3:
                    foreach (var consumer in runningDummyConsumers)
                    {
                        await consumer.DisposeAsync();
                    }

                    runningDummyConsumers.Clear();
                    AnsiConsole.MarkupLine("3 - All consumers stopped!");
                    break;

                case ConsoleKey.D4:
                    RabbitManagementService rms = new("http://localhost:15672/api", "guest", "guest");
                    var res = await rms.GetQueueInfoAsync("edi.orders.incoming");
                    AnsiConsole.MarkupLine($"MSG: {res.Messages} | MSG RDY: {res.MessagesReady} | MSG UNACK: {res.MessagesUnacknowledged}");
                    break;

                default:
                    AnsiConsole.MarkupLine($"No command under {key.Key}");
                    break;
            }
        }
    });

    while (!quitProgramCancellationSource.IsCancellationRequested)
    {
        // todo: how to display ConsumerTag of the consumers, so I can list theyr names?
        var consumerCount = await tasksChannel.ConsumerCountAsync("edi.orders.incoming");
        var messagesCount = await tasksChannel.MessageCountAsync("edi.orders.incoming");
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[bold]Messages waiting in the edi.orders.incoming':[/]");
        AnsiConsole.MarkupLine($"- [green]{messagesCount} messages[/]");

        AnsiConsole.MarkupLine("[bold]Consumers connected to 'edi.orders.incoming':[/]");
        AnsiConsole.MarkupLine($"- [green]{consumerCount} consumer(s) connected[/]");

        AnsiConsole.MarkupLine("[bold]Consumers references':[/]");
        AnsiConsole.MarkupLine($"- [green]{runningDummyConsumers.Count} consumer(s) connected[/]");

        foreach (var consumer in runningDummyConsumers)
        {
            AnsiConsole.MarkupLine($"- ConsumerTag: [blue]{consumer.ConsumerTag}[/]");
        }

        RabbitManagementService rms = new("http://localhost:15672/api", "guest", "guest");
        var res = await rms.GetQueueInfoAsync("edi.orders.incoming");
        AnsiConsole.MarkupLine($"[bold] MSG: {res.Messages} | MSG RDY: {res.MessagesReady} | MSG UNACK: {res.MessagesUnacknowledged} [/]");

        AnsiConsole.Markup("[red]Press ESC key to quit program.[/]");
        await Task.Delay(1000, quitProgramCancellationSource.Token);
    }
}
catch (BrokerUnreachableException exn)
{
    AnsiConsole.MarkupLine($"[red]Error - Please check if Docker with RabbitMQ instance is running.[/]");
    AnsiConsole.MarkupLine($"[red]{exn.Message}[/]");
}
catch (Exception ex)
{
    AnsiConsole.MarkupLine($"[red]An unexpected error occurred:[/]");
    AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
}


