using TtrpgHelperBackend.Models.Resource;

namespace TtrpgHelperBackend.Models;

public class CharacterSkill
{
    public int CharacterId { get; set; }
    public int SkillId { get; set; }
    
    // Optional: Extra data for the join table
    public int SkillValue { get; set; } 
    public bool IsProficient { get; set; } = false;
    
    public Character Character { get; set; } = null!;
    public Skill Skill { get; set; }  = null!;
}