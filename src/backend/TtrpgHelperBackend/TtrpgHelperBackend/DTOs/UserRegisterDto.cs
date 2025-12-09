using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.DTOs;
public class UserRegisterDto
{
    [Required]
    [MinLength(3)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
    
    public bool IsAdminRequest { get; set; } = false; 
    
    public string AvatarUrl { get; set; } = string.Empty;
}