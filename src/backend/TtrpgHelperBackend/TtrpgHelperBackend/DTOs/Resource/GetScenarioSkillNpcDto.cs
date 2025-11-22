namespace TtrpgHelperBackend.DTOs.Resource;

public class GetScenarioNpcSkillDto
{
    public int Id { get; set; }
    
    public int NpcId { get; set; }

    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public int Value { get; set; }
}