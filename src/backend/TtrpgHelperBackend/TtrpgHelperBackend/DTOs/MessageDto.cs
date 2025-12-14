namespace TtrpgHelperBackend.DTOs;

public class MessageDto
{
    public int Id { get; set; }
    public string SenderName { get; set; } // Zamiast SenderId, często lepiej od razu wysłać nazwę
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
}