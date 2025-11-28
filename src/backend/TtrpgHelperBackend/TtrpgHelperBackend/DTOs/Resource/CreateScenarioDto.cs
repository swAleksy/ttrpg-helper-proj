namespace TtrpgHelperBackend.DTOs.Resource;

public class CreateScenarioDto
{
    public int CampaignId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
}