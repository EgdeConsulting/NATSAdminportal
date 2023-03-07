namespace vite_api.Dto;

public class MessageDto
{
    public string Subject { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Acknowledgement { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public string Payload { get; set; }
}
