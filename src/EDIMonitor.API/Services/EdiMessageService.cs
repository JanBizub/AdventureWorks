using EDIMonitor.API.Models;
using System.Collections.Concurrent;

namespace EDIMonitor.API.Services;

public interface IEdiMessageService
{
    Task AddMessage(EdiMessage message);
    Task<IEnumerable<EdiMessageSummary>> GetRecentMessages(int count = 50);
    Task<EdiMessage?> GetMessage(int id);
}

public class EdiMessageService : IEdiMessageService
{
    private readonly ConcurrentDictionary<int, EdiMessage> _messages = new();
    private int _nextId = 1;

    public Task AddMessage(EdiMessage message)
    {
        message.Id = Interlocked.Increment(ref _nextId);
        _messages.TryAdd(message.Id, message);
        
        // Keep only the last 1000 messages in memory
        if (_messages.Count > 1000)
        {
            var oldestIds = _messages.Keys.OrderBy(x => x).Take(_messages.Count - 1000);
            foreach (var oldId in oldestIds)
            {
                _messages.TryRemove(oldId, out _);
            }
        }

        return Task.CompletedTask;
    }

    public Task<IEnumerable<EdiMessageSummary>> GetRecentMessages(int count = 50)
    {
        var recentMessages = _messages.Values
            .OrderByDescending(m => m.ReceivedAt)
            .Take(count)
            .Select(m => new EdiMessageSummary
            {
                Id = m.Id,
                ReceivedAt = m.ReceivedAt,
                MessageType = m.MessageType,
                Length = m.Length,
                Status = m.Status,
                SourceIdentifier = m.SourceIdentifier
            });

        return Task.FromResult(recentMessages);
    }

    public Task<EdiMessage?> GetMessage(int id)
    {
        _messages.TryGetValue(id, out var message);
        return Task.FromResult(message);
    }
}
