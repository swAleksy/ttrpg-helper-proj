namespace TtrpgHelperBackend.DTOs.Resource.Item;

public class GetScenarioItemDto
{
    public int Id { get; set; }
    
    public int? CampaignId { get; set; }

    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public string Type { get; set; } = string.Empty;
    
    public int Value  { get; set; }
}