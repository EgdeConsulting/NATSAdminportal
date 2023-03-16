﻿namespace vite_api.Dto;

public class MessageDataDto
{
    public List<MessageHeaderDTO> Headers { get; set; } = new();
    public string? Payload { get; set; }
    public string? Subject { get; set; }
}