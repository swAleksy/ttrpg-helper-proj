using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.DTOs.Session;

public class UpdateSessionDto
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } =  string.Empty;
    
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;
    
    [DataType(DataType.Date)]
    public DateTime ScheduledDate { get; set; }
    
    [MaxLength(20)]
    public string Status { get; set; } = "Planned";
}