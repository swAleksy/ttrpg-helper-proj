using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.Models.Resource;

public class Rule
{
    [Key]
    public int Id { get; set; }
    
    [MaxLength(30)]
    public string Category { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string ContentMarkdown { get; set; } = string.Empty;
}