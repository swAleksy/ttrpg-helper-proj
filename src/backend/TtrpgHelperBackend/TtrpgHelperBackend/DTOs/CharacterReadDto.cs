namespace TtrpgHelperBackend.DTOs;

public class CharacterReadDto
{
    public string Name { get; set; } = string.Empty;
    public int RaceId { get; set; }
    public int ClassId { get; set; }
    public int BackgroundId { get; set; }
    public int Level { get; set; } = 1;

    // Ability Scores (as we discussed, required inputs)
    public int Strength { get; set; }
    public int Dexterity { get; set; }
    public int Constitution { get; set; }
    public int Intelligence { get; set; }
    public int Wisdom { get; set; }
    public int Charisma { get; set; }
    
    public List<int> CharacterSkillsIds { get; set; } = new List<int>();
}