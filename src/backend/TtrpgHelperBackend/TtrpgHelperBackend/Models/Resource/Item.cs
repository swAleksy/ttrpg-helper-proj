using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TtrpgHelperBackend.Models.Session;

namespace TtrpgHelperBackend.Models.Resource;

public class Item
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [ForeignKey(nameof(Campaign))]
    public int CampaignId { get; set; }
    public Campaign Campaign { get; set; } = null!;
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? Type { get; set; }
    
    public int? Value { get; set; }
    
    public ICollection<ScenarioChapterItem> ChapterLinks { get; set; } = new  List<ScenarioChapterItem>();
}