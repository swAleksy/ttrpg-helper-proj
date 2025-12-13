using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.DTOs.Resource.Npc;

public class UpdateNpcSkillDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Value { get; set; }
}