namespace TtrpgHelperBackend.DTOs;

public class NotificationDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public string Type { get; set; } // Np. "FriendRequest", "System"
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? FromUserId { get; set; }
}