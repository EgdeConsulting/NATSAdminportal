namespace vite_api.Dto;

public class BasicStreamInfoDto
{
    public string Name { get; set; } = null!;
    public long SubjectCount { get; set; }
    public long ConsumerCount { get; set; }
    public ulong MessageCount { get; set; }
}
