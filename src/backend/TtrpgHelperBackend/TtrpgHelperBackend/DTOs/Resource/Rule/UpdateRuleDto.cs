using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.DTOs.Resource.Rule;

public class UpdateRuleDto
{
    [Required]
    public int Id { get; set; }
    
    [MaxLength(30)]
    public string Category { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string ContentMarkdown { get; set; } = string.Empty;
}