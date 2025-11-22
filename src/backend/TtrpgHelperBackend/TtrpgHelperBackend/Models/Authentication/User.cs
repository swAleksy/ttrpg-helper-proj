using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.Models.Authentication;

public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string UserName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public byte[] PasswordHash { get; set; } = new byte[0];
    
    [Required]
    public byte[] PasswordSalt { get; set; } = new byte[0];
    
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public string AvatarUrl { get; set; } = "/uploads/avatars/default.jpeg";
    
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}