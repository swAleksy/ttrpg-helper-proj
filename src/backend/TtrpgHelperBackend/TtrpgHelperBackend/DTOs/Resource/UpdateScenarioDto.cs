namespace TtrpgHelperBackend.DTOs.Resource;

public class UpdateScenarioDto
{
    public int Id { get; set; }
    
    public int CampaignId { get; set; }
    
    public string Name { get; set; }
    
    public string?  Description { get; set; }
}