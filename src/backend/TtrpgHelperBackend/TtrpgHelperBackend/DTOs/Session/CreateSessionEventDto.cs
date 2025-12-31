using System.ComponentModel.DataAnnotations;
using TtrpgHelperBackend.Models.Session;

namespace TtrpgHelperBackend.DTOs.Session;

public class CreateSessionEventDto
{
    [Required]
    public int SessionId { get; set; }
    
    [Required]
    public SessionEventType Type { get; set; } 
    public string DataJson { get; set; } = string.Empty;
}
