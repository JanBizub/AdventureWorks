namespace RabbitMQ.Utils;

using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

// https://rawcdn.githack.com/rabbitmq/rabbitmq-management/v3.8.9/priv/www/api/index.html

// TODO: add ILogger and replace console.logs
// TODO: replace username / password with something more 21 century-like
public class RabbitManagementService(string managementApiUrl, string username, string password)
{
    private readonly HttpClient _httpClient = CreateHttpClient(username, password);

    private static HttpClient CreateHttpClient(string username, string password)
    {
        var client = new HttpClient();
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
        return client;
    }

    public async Task<QueueInfo?> GetQueueInfoAsync(string queueName, string vhost = "/")
    {
        try
        {
            var encodedVhost = Uri.EscapeDataString(vhost);
            var encodedQueueName = Uri.EscapeDataString(queueName);
            var url = $"{managementApiUrl}/queues/{encodedVhost}/{encodedQueueName}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };

            return JsonSerializer.Deserialize<QueueInfo>(json, options);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting queue info: {ex.Message}");
            return null;
        }
    }

    public async Task<List<QueueInfo>> GetAllQueuesAsync(string vhost = "/")
    {
        try
        {
            var encodedVhost = Uri.EscapeDataString(vhost);
            var url = $"{managementApiUrl}/queues/{encodedVhost}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new List<QueueInfo>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };

            return JsonSerializer.Deserialize<List<QueueInfo>>(json, options) ?? new List<QueueInfo>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting all queues: {ex.Message}");
            return new List<QueueInfo>();
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}

public class QueueInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("vhost")]
    public string VHost { get; set; } = string.Empty;

    [JsonPropertyName("durable")]
    public bool Durable { get; set; }

    [JsonPropertyName("auto_delete")]
    public bool AutoDelete { get; set; }

    [JsonPropertyName("messages")]
    public int Messages { get; set; }

    [JsonPropertyName("messages_ready")]
    public int MessagesReady { get; set; }

    [JsonPropertyName("messages_unacknowledged")]
    public int MessagesUnacknowledged { get; set; }

    [JsonPropertyName("consumers")]
    public int Consumers { get; set; }

    [JsonPropertyName("memory")]
    public long Memory { get; set; }

    [JsonPropertyName("message_stats")]
    public MessageStats? MessageStats { get; set; }

    [JsonPropertyName("node")]
    public string Node { get; set; } = string.Empty;

    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;
}

public class MessageStats
{
    [JsonPropertyName("publish")]
    public int Publish { get; set; }

    [JsonPropertyName("deliver")]
    public int Deliver { get; set; }

    [JsonPropertyName("deliver_no_ack")]
    public int DeliverNoAck { get; set; }

    [JsonPropertyName("get")]
    public int Get { get; set; }

    [JsonPropertyName("get_no_ack")]
    public int GetNoAck { get; set; }

    [JsonPropertyName("deliver_get")]
    public int DeliverGet { get; set; }

    [JsonPropertyName("redeliver")]
    public int Redeliver { get; set; }

    [JsonPropertyName("ack")]
    public int Ack { get; set; }

    [JsonPropertyName("publish_details")]
    public RateDetails? PublishDetails { get; set; }

    [JsonPropertyName("deliver_details")]
    public RateDetails? DeliverDetails { get; set; }

    [JsonPropertyName("ack_details")]
    public RateDetails? AckDetails { get; set; }
}

public class RateDetails
{
    [JsonPropertyName("rate")]
    public double Rate { get; set; }

    [JsonPropertyName("avg")]
    public double Avg { get; set; }

    [JsonPropertyName("avg_rate")]
    public double AvgRate { get; set; }
}