using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.Models;

public class Notification
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }
    
    public NotificationType? Type { get; set; }

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Message { get; set; } = string.Empty;
    
    public int? FromUserId { get; set; }

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
}

public enum NotificationType
{
    AddedToGroup = 1,
    NewMessage = 2,
    FriendRequest = 3,       // <--- DODANE
    FriendRequestAccepted = 4 // <--- DODANE
}