using EDIGateway.API.Models;
using System.Text.Json;

namespace EDIGateway.API.Services;

public interface IEdiNotificationService
{
    Task NotifyMessageReceived(EdiMessage message);
}

public class EdiNotificationService : IEdiNotificationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EdiNotificationService> _logger;
    private readonly string _ediMonitorApiUrl;

    public EdiNotificationService(HttpClient httpClient, ILogger<EdiNotificationService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _ediMonitorApiUrl = configuration.GetValue<string>("EdiMonitorApi:BaseUrl") ?? "https://localhost:7236";
    }

    public async Task NotifyMessageReceived(EdiMessage message)
    {
        try
        {
            var json = JsonSerializer.Serialize(message);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_ediMonitorApiUrl}/api/EdiMonitor/message-received", content);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to notify EDI Monitor API. Status: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying EDI Monitor API");
        }
    }
}
