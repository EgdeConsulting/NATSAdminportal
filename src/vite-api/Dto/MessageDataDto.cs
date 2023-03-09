namespace vite_api.Dto;

public class MessageDataDto
{
    public Dictionary<string, string> Headers { get; set; } = new();
    public string? Payload { get; set; }
    public string? Subject { get; set; }
}