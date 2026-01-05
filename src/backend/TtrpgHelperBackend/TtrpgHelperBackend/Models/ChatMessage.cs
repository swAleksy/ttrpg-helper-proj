using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TtrpgHelperBackend.Models.Authentication;

namespace TtrpgHelperBackend.Models;

public class ChatMessage
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int SenderId { get; set; }
    
    [ForeignKey("SenderId")]
    public virtual User Sender { get; set; } = null!; 

    [Required]
    [MaxLength(1000)] 
    public string Content { get; set; } = string.Empty;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;

    // PRYWATNA WIADOMOŚĆ 
    public int? ReceiverId { get; set; }

    [ForeignKey("ReceiverId")]
    public virtual User? Receiver { get; set; } 

    // CZAT GRUPOWY / SESJA GRY 
    public string? SessionId { get; set; } 
}