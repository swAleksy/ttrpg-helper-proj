using System.ComponentModel.DataAnnotations;
using TtrpgHelperBackend.Models.Authentication;

namespace TtrpgHelperBackend.DTOs;

public class UserInfoDto
{
    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string AvatarUrl  { get; set; } = string.Empty;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}