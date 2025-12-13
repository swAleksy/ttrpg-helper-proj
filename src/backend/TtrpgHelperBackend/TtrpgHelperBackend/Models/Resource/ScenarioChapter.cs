using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TtrpgHelperBackend.Models.Resource;

public class ScenarioChapter
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey(nameof(Scenario))]
    public int ScenarioId { get; set; }

    public Scenario Scenario { get; set; } = null!;
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    public string ContentMarkdown { get; set; } = string.Empty;
    
    public ICollection<ScenarioChapterNote> Notes { get; set; } = new  List<ScenarioChapterNote>();
    public ICollection<ScenarioChapterNpc> Npcs { get; set; } = new   List<ScenarioChapterNpc>();
    public ICollection<ScenarioChapterItem> Items { get; set; } = new   List<ScenarioChapterItem>();
    public ICollection<ScenarioChapterLocation> Locations { get; set; } = new   List<ScenarioChapterLocation>();
}
