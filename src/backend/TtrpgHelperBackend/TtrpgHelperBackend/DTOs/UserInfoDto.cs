using System.ComponentModel.DataAnnotations;
using TtrpgHelperBackend.Models.Authentication;

namespace TtrpgHelperBackend.DTOs;

public class UserInfoDto
{
    public int Id { get; set; } 
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AvatarUrl  { get; set; } = string.Empty;

}