using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.DTOs.Resource.Background;

public class UpdateBackgroundDto
{
    [Required]
    public int  Id { get; set; }
    
    [MaxLength(30)]
    public string Name  { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;
}