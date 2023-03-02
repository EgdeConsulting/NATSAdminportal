using System.Text.Json.Nodes;

namespace vite_api.Dto;

public class PublishMessageDto
{
    public string Subject { get; set; } = null!;
    public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
    public string Payload { get; set; }
}
