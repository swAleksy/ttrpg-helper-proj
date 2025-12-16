namespace TtrpgHelperBackend.Models.Authentication;

public class Friendship
{
    // Kto zaprosił (powiązane z listą User.Friends)
    public int SourceUserId { get; set; }
    public User SourceUser { get; set; } = null!;

    // Kogo zaproszono (powiązane z listą User.FriendOf)
    public int TargetUserId { get; set; }
    public User TargetUser { get; set; } = null!;

    // Status jest kluczowy: bez niego każdy, komu wyślesz zaproszenie, 
    // od razu byłby Twoim "znajomym" bez jego zgody.
    public FriendshipStatus Status { get; set; } 
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum FriendshipStatus
{
    Pending,
    Accepted,
    Blocked
}
