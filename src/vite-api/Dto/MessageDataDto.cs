namespace vite_api.Dto;

public class MessageDataDto
{
    public List<MessageHeaderDto> Headers { get; set; } = new();
    public MessagePayloadDto Payload { get; set; } = new();
    public string? Subject { get; set; }
}