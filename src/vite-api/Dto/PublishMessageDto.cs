using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;

namespace vite_api.Dto;

public class PublishMessageDto
{
    public string Subject { get; set; }
    public List<MessageHeaderDTO> Headers { get; set; } = new();

    public string Payload { get; set; }
}
