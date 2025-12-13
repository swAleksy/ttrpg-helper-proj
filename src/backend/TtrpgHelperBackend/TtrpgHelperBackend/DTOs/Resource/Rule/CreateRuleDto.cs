using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.DTOs.Resource.Rule;

public class CreateRuleDto
{
    [MaxLength(30)]
    public string Category { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string ContentMarkdown { get; set; } = string.Empty;
}