using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TtrpgHelperBackend.Models.Resource;

public class NpcSkill
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [ForeignKey(nameof(Npc))]
    public int NpcId { get; set; }
    public Npc Npc { get; set; } = null!;
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public int Value { get; set; }
}
