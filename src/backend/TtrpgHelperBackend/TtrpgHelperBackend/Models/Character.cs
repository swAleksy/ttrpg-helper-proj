using System.ComponentModel.DataAnnotations;
using TtrpgHelperBackend.Models.Authentication;

namespace TtrpgHelperBackend.Models;

public class Character
{
    [Key]
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = new User();
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    
    // 🔑 Foreign Key: Links Character to Races
    public int RaceId { get; set; }
    
    // ➡️ Navigation Property: The actual Race object
    // EF Core convention: Looks for a property named RaceId or Race_id
    public Race Race { get; set; } 
    
    public int ClassId { get; set; }
    public Class Class { get; set; } 
    
    public int BackgroundId { get; set; }
    public Background Background { get; set; } 
    
    public int Level { get; set; }
    
    public int Strength { get; set; }
    
    public int Dexterity { get; set; }
    
    public int Constitution { get; set; }
    
    public int Intelligence { get; set; }
    
    public int Wisdom { get; set; }
    
    public int Charisma { get; set; }
    
    public ICollection<CharacterSkill> CharacterSkills { get; set; } = new List<CharacterSkill>();
}