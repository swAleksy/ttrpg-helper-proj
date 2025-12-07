namespace TtrpgHelperBackend.DTOs.Resource.Note;

public class GetScenarioNoteDto
{
    public int Id { get; set; }
    
    public int CampaignId { get; set; }

    public string Name { get; set; } = string.Empty;
    
    public string ContentMarkdown { get; set; } = string.Empty;
}