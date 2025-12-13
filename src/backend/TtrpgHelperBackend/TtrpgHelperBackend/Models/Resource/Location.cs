using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TtrpgHelperBackend.Models.Session;

namespace TtrpgHelperBackend.Models.Resource;

public class Location
{
    [Key]
    public int Id { get; set; }

    public bool IsCompendium { get; set; } = false;
    
    [ForeignKey(nameof(Campaign))]
    public int? CampaignId { get; set; }
    public Campaign? Campaign { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? Region { get; set; }

    public string Description { get; set; } = string.Empty;

    public ICollection<ScenarioChapterLocation> ChapterLinks { get; set; } = new List<ScenarioChapterLocation>();
}