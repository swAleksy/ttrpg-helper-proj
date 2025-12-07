using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.DTOs.Resource.Class;

public class UpdateClassDto
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(30)]
    public string Name  { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;
}
