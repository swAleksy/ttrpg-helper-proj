using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.DTOs.Session;

public class CreateCampaignDto
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    public int GameMasterId { get; set; }
}