namespace TtrpgHelperBackend.DTOs.Resource;

public class GetScenarioDto
{
    public int Id { get; set; }
    
    public int CampaignId { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;

    public IEnumerable<GetScenarioChapterDto> Chapters { get; set; } = new List<GetScenarioChapterDto>();
}
