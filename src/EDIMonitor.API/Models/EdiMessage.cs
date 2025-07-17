namespace EDIMonitor.API.Models;

public class EdiMessage
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }
    public string MessageType { get; set; } = "Unknown";
    public int Length { get; set; }
    public string Status { get; set; } = "Received";
    public string? SourceIdentifier { get; set; }
}

public class EdiMessageSummary
{
    public int Id { get; set; }
    public DateTime ReceivedAt { get; set; }
    public string MessageType { get; set; } = "Unknown";
    public int Length { get; set; }
    public string Status { get; set; } = "Received";
    public string? SourceIdentifier { get; set; }
}
