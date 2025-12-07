using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.DTOs.Calendar;

public class UpdateCalendarEventDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime EventDate { get; set; }
}
