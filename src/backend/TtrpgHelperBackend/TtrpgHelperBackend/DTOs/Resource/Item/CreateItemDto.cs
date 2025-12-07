using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.DTOs.Resource.Item;

public class CreateItemDto
{
    public int? CampaignId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string  Description { get; set; } = string.Empty;

    [MaxLength(30)] public string? Type { get; set; }
    
    public int? Value  { get; set; }
}