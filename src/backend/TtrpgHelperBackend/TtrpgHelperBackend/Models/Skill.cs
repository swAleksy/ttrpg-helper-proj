using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.Models;

public class Skill
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty; // Renamed to 'Name' for clarity
    
    public string Description { get; set; } = string.Empty;
    
    // ➡️ Navigation Property: Collection of the join table entities
    public ICollection<CharacterSkill> CharacterSkills { get; set; } = new List<CharacterSkill>();
}