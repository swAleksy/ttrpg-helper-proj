using TtrpgHelperBackend.Models;

namespace TtrpgHelperBackend.DTOs.Resource;

public class GetScenarioNpcDto
{
    public int Id { get; set; }
    
    public int CampaignId { get; set; }

    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;

    public string Race { get; set; } = string.Empty;
    
    public string Class { get; set; }  = string.Empty;
    
    public int Level { get; set; }
    
    public int Strength { get; set; }
    public int Dexterity { get; set; }
    public int Constitution { get; set; }
    public int Intelligence { get; set; }
    public int Wisdom { get; set; }
    public int Charisma { get; set; }

    public IEnumerable<GetScenarioNpcSkillDto> Skills { get; set; } = new List<GetScenarioNpcSkillDto>();
}