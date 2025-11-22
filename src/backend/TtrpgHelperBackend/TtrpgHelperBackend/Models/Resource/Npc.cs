using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TtrpgHelperBackend.Models.Session;

namespace TtrpgHelperBackend.Models.Resource;

public class Npc
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

    public string Description { get; set; } = string.Empty;
    
    public int? RaceId { get; set; }
    public Race? Race { get; set; }
    
    public int? ClassId { get; set; }
    public Class? Class { get; set; }
    
    public int Level { get; set; }
    
    public int Strength { get; set; }
    public int Dexterity { get; set; }
    public int Constitution { get; set; }
    public int Intelligence { get; set; }
    public int Wisdom { get; set; }
    public int Charisma { get; set; }
    
    public ICollection<NpcSkill> Skills { get; set; } = new List<NpcSkill>();

    public ICollection<ScenarioChapterNpc> ChapterLinks { get; set; } = new List<ScenarioChapterNpc>();
}
