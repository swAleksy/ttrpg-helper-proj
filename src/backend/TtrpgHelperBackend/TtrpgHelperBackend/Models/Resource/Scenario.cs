using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TtrpgHelperBackend.Models.Session;

namespace TtrpgHelperBackend.Models.Resource;

public class Scenario
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [ForeignKey(nameof(Campaign))]
    public int CampaignId { get; set; }
    public Campaign Campaign { get; set; } =  null!;
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    public ICollection<ScenarioChapter> Chapters { get; set; } = new List<ScenarioChapter>();
}