using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.Models.Authentication;

public class Role
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string Name { get; set; } = string.Empty;
    
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}