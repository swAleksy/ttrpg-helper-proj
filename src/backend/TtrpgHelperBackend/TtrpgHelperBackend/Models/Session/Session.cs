using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TtrpgHelperBackend.Models.Authentication;

namespace TtrpgHelperBackend.Models.Session;

public class Session
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } =  string.Empty;
    
    [MaxLength(200)]
    public string? Description { get; set; } = string.Empty;
    
    [Required]
    [DataType(DataType.Date)]
    public DateTime ScheduledDate { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = string.Empty;
    
    [Required]
    [ForeignKey(nameof(User))]
    public int GameMasterId { get; set; }
    public User GameMaster { get; set; } =  null!;
    
    [InverseProperty(nameof(SessionPlayer.Session))]
    public ICollection<SessionPlayer> Players  { get; set; } = new List<SessionPlayer>();
    
    //todo - czy nie byłoby dobrze dodać jeszcze CreatedAt oraz UpdatedAt ?
}