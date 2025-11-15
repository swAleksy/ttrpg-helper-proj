namespace TtrpgHelperBackend.DTOs.Session;

public class GetCampaignDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = "";
    
    public string? Description { get; set; }
    
    public IEnumerable<GetSessionDto> Sessions { get; set; } = new List<GetSessionDto>();
}
