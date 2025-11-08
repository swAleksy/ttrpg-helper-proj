namespace TtrpgHelperBackend.DTOs.Session;

public class SummarizeSessionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
}