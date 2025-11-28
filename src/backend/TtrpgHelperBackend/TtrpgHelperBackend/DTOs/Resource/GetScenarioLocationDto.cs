using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.DTOs.Resource;

public class GetScenarioLocationDto
{
    public int Id { get; set; } 
    
    public int CampaignId { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Region { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
}