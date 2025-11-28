using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.DTOs.Session;

public class UpdateSessionDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } =  string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public DateTime ScheduledDate { get; set; }
    
    public string Status { get; set; } = "Planned";
}