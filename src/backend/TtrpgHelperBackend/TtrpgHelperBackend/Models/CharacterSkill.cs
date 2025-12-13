using TtrpgHelperBackend.Models.Resource;

namespace TtrpgHelperBackend.Models;

public class CharacterSkill
{
    // 🔑 Composite Foreign Keys
    public int CharacterId { get; set; }
    public int SkillId { get; set; }
    
    // Optional: Extra data for the join table
    public int SkillValue { get; set; } // e.g., the final modifier (+5)
    public bool IsProficient { get; set; } = false;
    
    // ➡️ Navigation Properties
    public Character Character { get; set; } = null!;
    public Skill Skill { get; set; }  = null!;
}