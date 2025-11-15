using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TtrpgHelperBackend.Models.Authentication;

namespace TtrpgHelperBackend.Models.Session;

public class SessionPlayer
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [ForeignKey(nameof(Player))]
    public int PlayerId { get; set; }
    public User Player { get; set; } = null!;
    
    [Required]
    [ForeignKey(nameof(Session))]
    public int SessionId { get; set; }
    public Session Session { get; set; }  = null!;
}
