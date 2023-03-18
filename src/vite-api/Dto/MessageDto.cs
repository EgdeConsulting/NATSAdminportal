namespace vite_api.Dto;

public class MessageDto
{
    public ulong SequenceNumber { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Stream { get; set; }
    public string? Subject { get; set; }
    public bool Erase { get; set; }
}
