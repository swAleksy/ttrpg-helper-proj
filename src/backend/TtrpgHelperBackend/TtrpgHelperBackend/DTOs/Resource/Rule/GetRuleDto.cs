namespace TtrpgHelperBackend.DTOs.Resource.Rule;

public class GetRuleDto
{
    public int Id { get; set; }
    
    public string Category { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    
    public string ContentMarkdown { get; set; } = string.Empty;
}