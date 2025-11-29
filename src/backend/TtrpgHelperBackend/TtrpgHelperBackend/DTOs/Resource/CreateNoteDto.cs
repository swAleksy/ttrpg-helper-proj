using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.DTOs.Resource;

public class CreateNoteDto
{
    [Required]
    public int CampaignId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    public string ContentMarkdown { get; set; } = string.Empty;
}