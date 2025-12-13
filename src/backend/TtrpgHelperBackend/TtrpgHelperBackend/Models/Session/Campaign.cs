using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TtrpgHelperBackend.Models.Authentication;
using TtrpgHelperBackend.Models.Resource;

namespace TtrpgHelperBackend.Models.Session;

public class Campaign
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "Active";
    
    [Required]
    [ForeignKey(nameof(User))]
    public int GameMasterId { get; set; }
    public User GameMaster { get; set; } = null!;
    
    public ICollection<Session> Sessions { get; set; } = new  List<Session>();
    public ICollection<Scenario> Scenarios { get; set; } = new List<Scenario>();

}
