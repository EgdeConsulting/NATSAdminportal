namespace vite_api.Dto;

public class ExtendedStreamInfoDto
{
    public string Name { get; set; } = null!;
    public List<string> Subjects { get; set; } = new List<string>();
    public List<string> Consumers { get; set; } = new List<string>();
    public string? Description { get; set; }
    public ulong Messages { get; set; }
    public long Deleted { get; set; }
    public PoliciesDto Policies { get; set; } = new PoliciesDto();
}
