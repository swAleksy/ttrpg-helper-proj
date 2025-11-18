using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.Models;

public class Notification
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = default!;
    
    public NotificationType Type { get; set; }

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = default!;

    [Required]
    [MaxLength(500)]
    public string Message { get; set; } = default!;
    
    public string? FromUserId { get; set; }

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
}

public enum NotificationType
{
    AddedToGroup = 1,
    NewMessage = 2
}