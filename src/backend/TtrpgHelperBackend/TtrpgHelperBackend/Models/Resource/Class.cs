using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.Models.Resource;

public class Class
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(30)]
    public string Name  { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;
    
    public ICollection<Character> Characters { get; set; } = new List<Character>();
}