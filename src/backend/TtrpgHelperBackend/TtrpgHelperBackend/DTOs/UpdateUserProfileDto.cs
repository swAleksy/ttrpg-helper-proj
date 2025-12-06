using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.DTOs;

public class UpdateUserProfileDto
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
}