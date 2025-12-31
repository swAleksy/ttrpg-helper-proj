using System.Text.Json.Serialization;
using TtrpgHelperBackend.Models.Session;

namespace TtrpgHelperBackend.DTOs.Session;

public class GetSessionEventDto
{
    public int Id { get; set; }
    
    public int SessionId { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SessionEventType Type { get; set; }
    
    public string DataJson { get; set; } = string.Empty;
    
    public DateTime Timestamp { get; set; }
    
    public int UserId { get; set; }
    
    public string UserName { get; set; } = string.Empty;
}
