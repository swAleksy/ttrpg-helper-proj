using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TtrpgHelperBackend.Models.Authentication;

namespace TtrpgHelperBackend.Models.Session;

public class SessionEvent
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [ForeignKey(nameof(Session))]
    public int SessionId { get; set; }
    public Session Session { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    [Required]
    [MaxLength(50)]
    public SessionEventType Type { get; set; }
    
    public string DataJson { get; set; } = string.Empty;
    
    [Required]
    [DataType(DataType.DateTime)]
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

public enum SessionEventType
{
    ChatMessage,
    DiceRoll,
    ShareItem,
    ShareNpc,
    UserJoined,
    UserLeft
}
