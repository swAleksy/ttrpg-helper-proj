using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.Models;

public class Background
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name  { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public ICollection<Character> Characters { get; set; } = new List<Character>();
}