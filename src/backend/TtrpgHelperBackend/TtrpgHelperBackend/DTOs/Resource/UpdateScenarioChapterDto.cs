using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.DTOs.Resource;

public class UpdateScenarioChapterDto
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    public int ScenarioId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } =  string.Empty;
    
    [Required]
    public string ContentMarkdown { get; set; } = string.Empty;
}