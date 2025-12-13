using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.DTOs.Resource.Location;

public class CreateCompendiumLocationDto
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? Region { get; set; }

    public string Description { get; set; } = string.Empty;
}