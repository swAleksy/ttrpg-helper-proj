namespace TtrpgHelperBackend.DTOs.Session;

public class CreateSessionEventDto
{
    public int SessionId { get; set; }
    
    public string Type { get; set; } = string.Empty;

    public string DataJson { get; set; } = string.Empty;
}
