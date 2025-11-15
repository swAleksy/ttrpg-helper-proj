using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.DTOs.Session;

public class CreateSessionDto
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } =  string.Empty;
    
    [MaxLength(200)]
    public string? Description { get; set; } = string.Empty;
    
    [DataType(DataType.Date)]
    public DateTime ScheduledDate { get; set; }
    
    [MaxLength(20)]
    public string Status { get; set; } = "Planned";

    [Required]
    public int GameMasterId { get; set; }
    
    [Required]
    public int CampaignId { get; set; }
}