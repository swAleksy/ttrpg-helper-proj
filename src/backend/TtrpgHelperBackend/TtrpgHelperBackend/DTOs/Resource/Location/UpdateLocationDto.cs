using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TtrpgHelperBackend.Models.Session;

namespace TtrpgHelperBackend.DTOs.Resource.Location;

public class UpdateLocationDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? Region { get; set; }
    
    public string Description { get; set; } = string.Empty;
}