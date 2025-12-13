using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TtrpgHelperBackend.Models.Session;

namespace TtrpgHelperBackend.Models.Resource;

public class Note
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
    
    public string ContentMarkdown { get; set; } = string.Empty;

    public ICollection<ScenarioChapterNote> ChapterLinks { get; set; } = new  List<ScenarioChapterNote>();
}
