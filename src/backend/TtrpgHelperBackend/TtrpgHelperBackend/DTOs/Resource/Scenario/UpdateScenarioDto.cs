using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.DTOs.Resource.Scenario;

public class UpdateScenarioDto
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    public int CampaignId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    
    [MaxLength(200)]
    public string?  Description { get; set; }
}