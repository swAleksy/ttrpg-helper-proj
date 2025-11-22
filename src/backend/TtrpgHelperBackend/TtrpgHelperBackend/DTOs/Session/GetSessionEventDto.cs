namespace TtrpgHelperBackend.DTOs.Session;

public class GetSessionEventDto
{
    public int Id { get; set; }
    
    public int SessionId { get; set; }
    
    public string Type { get; set; } = string.Empty;
    
    public string DataJson { get; set; } = string.Empty;
    
    public DateTime Timestamp { get; set; }
    
    public int UserId { get; set; }
    
    public string UserName { get; set; } = string.Empty;
}
