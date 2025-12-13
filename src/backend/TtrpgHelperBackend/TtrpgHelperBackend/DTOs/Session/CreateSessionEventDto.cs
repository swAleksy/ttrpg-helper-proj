using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.DTOs.Session;

public class CreateSessionEventDto
{
    [Required]
    public int SessionId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty;
    
    public string DataJson { get; set; } = string.Empty;
}
