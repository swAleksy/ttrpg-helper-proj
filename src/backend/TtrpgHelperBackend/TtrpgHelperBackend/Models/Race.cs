using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.Models;

public class Race
{
    [Key] // Assuming you need a [Key] annotation here
    public int Id { get; set; }
    
    [Required]
    public string Name  { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    // ➡️ Navigation Property: The collection of Characters of this Race
    public ICollection<Character> Characters { get; set; } = new List<Character>();
}
