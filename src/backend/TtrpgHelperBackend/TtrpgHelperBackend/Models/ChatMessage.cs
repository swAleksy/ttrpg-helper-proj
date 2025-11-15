namespace TtrpgHelperBackend.Models;

public class ChatMessage
{
    public int Id { get; set; }
    public string SenderId { get; set; }
    public string? ReceiverId { get; set; }  // null for team chat
    public string? SessionId { get; set; }   // null for private chat
    public string Content { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;
}