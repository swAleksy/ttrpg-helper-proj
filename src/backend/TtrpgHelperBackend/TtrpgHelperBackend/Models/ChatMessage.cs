using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TtrpgHelperBackend.Models.Authentication;

namespace TtrpgHelperBackend.Models;

public class ChatMessage
{
    [Key]
    public int Id { get; set; }

    // 1. KTO WYSŁAŁ (Zawsze wymagane)
    [Required]
    public int SenderId { get; set; }
    
    [ForeignKey("SenderId")]
    public virtual User Sender { get; set; } = null!; // Pozwala na .Include(m => m.Sender)

    // 2. TREŚĆ
    [Required]
    [MaxLength(1000)] // Warto dać limit
    public string Content { get; set; } = string.Empty;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;

    // --- OPCJA A: PRYWATNA WIADOMOŚĆ (DM) ---
    // Nullable, bo w czacie grupowym to będzie null
    public int? ReceiverId { get; set; }

    [ForeignKey("ReceiverId")]
    public virtual User? Receiver { get; set; } // Pozwala pobrać dane odbiorcy

    // --- OPCJA B: CZAT GRUPOWY / SESJA GRY ---
    // Nullable, bo w prywatnym czacie to będzie null
    // UWAGA: Upewnij się czy SessionId to int czy string/Guid w Twoim systemie gry!
    public string? SessionId { get; set; } 
}