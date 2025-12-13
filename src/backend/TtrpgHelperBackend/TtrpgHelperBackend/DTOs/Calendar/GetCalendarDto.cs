namespace TtrpgHelperBackend.DTOs.Calendar;

public class GetCalendarDto
{
    public int Id { get; set; }

    public string Type { get; set; } = string.Empty;
    
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public DateTime Date { get; set; }
    
    public int? CampaignId  { get; set; }
    
    public int? SessionId { get; set; }
}
