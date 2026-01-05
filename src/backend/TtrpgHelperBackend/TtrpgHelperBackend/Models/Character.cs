using System.ComponentModel.DataAnnotations;
using TtrpgHelperBackend.Models.Authentication;
using TtrpgHelperBackend.Models.Resource;

namespace TtrpgHelperBackend.Models;

public class Character
{
    [Key]
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    
    public int RaceId { get; set; }
    
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