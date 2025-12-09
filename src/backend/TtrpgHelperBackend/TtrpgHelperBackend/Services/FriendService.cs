using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs;
using TtrpgHelperBackend.Models.Authentication;
using TtrpgHelperBackend.Helpers;

namespace TtrpgHelperBackend.Services;

public interface IFriendService
{
    Task<ServiceResponseH<bool>> SendFriendRequestAsync(int sourceUserId, int targetUserId);
    Task<ServiceResponseH<bool>> AcceptFriendRequestAsync(int currentUserId, int requesterUserId);
    Task<ServiceResponseH<bool>> RemoveFriendshipAsync(int userId, int friendId);
    Task<IEnumerable<UserInfoDto>> GetFriendsAsync(int userId);
    Task<IEnumerable<UserInfoDto>> GetPendingRequestsAsync(int userId);
}

public class FriendService : IFriendService
{
    private readonly ApplicationDbContext _context;
    
    public FriendService(ApplicationDbContext context)
    {
        _context = context;
    }

    // 1. Wysyłanie zaproszenia (zabezpieczone przed dublowaniem)
    public async Task<ServiceResponseH<bool>> SendFriendRequestAsync(int sourceUserId, int targetUserId)
    {
        if (sourceUserId == targetUserId)
        {
            return new ServiceResponseH<bool> { Message = "You cannot add yourself.", Success = false }; 
        }

        var existingFriendship = await _context.Friendships
            .FirstOrDefaultAsync(f => 
                (f.SourceUserId == sourceUserId && f.TargetUserId == targetUserId) ||
                (f.SourceUserId == targetUserId && f.TargetUserId == sourceUserId));

        if (existingFriendship != null)
        {
            return new ServiceResponseH<bool> { Message = "Relationship already exists.", Success = false }; 
        }

        var friendship = new Friendship
        {
            SourceUserId = sourceUserId,
            TargetUserId = targetUserId,
            Status = FriendshipStatus.Pending
        };

        _context.Friendships.Add(friendship);
        await _context.SaveChangesAsync();
        
        return new ServiceResponseH<bool> { Message = "Friend request sent.", Success = true };
    }

    // 2. Akceptacja zaproszenia
    // requestingUserId -> ten kto wysłał zaproszenie (np. User1)
    // currentUserId -> ten kto klika "Akceptuj" (np. User2)
    public async Task<ServiceResponseH<bool>> AcceptFriendRequestAsync(int currentUserId, int requesterUserId){
        // Query Logic: Find a request sent BY requester TO me (current)
        var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f => 
                f.SourceUserId == requesterUserId && 
                f.TargetUserId == currentUserId && 
                f.Status == FriendshipStatus.Pending);

        if (friendship == null) 
        {
            return new ServiceResponseH<bool> { Message = "Friend request not found.", Success = false }; 
        }

        friendship.Status = FriendshipStatus.Accepted;
        await _context.SaveChangesAsync();
        
        return new ServiceResponseH<bool> { Message = "Friend request accepted.", Success = true }; 
    }

    public async Task<IEnumerable<UserInfoDto>> GetFriendsAsync(int userId)
    {
        // Pobieramy relacje, gdzie user jest po lewej LUB po prawej stronie ORAZ status to Accepted
        var friendships = await _context.Friendships
            .Include(f => f.SourceUser)
            .Include(f => f.TargetUser)
            .Where(f => (f.SourceUserId == userId || f.TargetUserId == userId) 
                        && f.Status == FriendshipStatus.Accepted)
            .ToListAsync();

        // Mapujemy na listę użytkowników "po drugiej stronie kabla"
        return friendships.Select(f => 
        {
            var friend = f.SourceUserId == userId ? f.TargetUser : f.SourceUser;
            return new UserInfoDto
            {
                Id = friend.Id,
                UserName = friend.UserName,
                AvatarUrl = friend.AvatarUrl,
                Email = friend.Email
            };
        });
    }

    public async Task<ServiceResponseH<bool>> RemoveFriendshipAsync(int userId, int friendId)
    {
        // Szukamy relacji w obie strony
        var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f => 
                (f.SourceUserId == userId && f.TargetUserId == friendId) ||
                (f.SourceUserId == friendId && f.TargetUserId == userId));

        if (friendship == null) 
        {
            return new ServiceResponseH<bool> { Message = "Relationship not found.", Success = false }; 
        }

        _context.Friendships.Remove(friendship);
        await _context.SaveChangesAsync();
        
        return new ServiceResponseH<bool> { Message = "Friend removed.", Success = true }; 
    }
    
    // 5. Pobieranie oczekujących zaproszeń (tylko te, które ktoś wysłał DO OBECNEGO USERA)
    public async Task<IEnumerable<UserInfoDto>> GetPendingRequestsAsync(int userId)
    {
        return await _context.Friendships
            .Where(f => f.TargetUserId == userId && f.Status == FriendshipStatus.Pending)
            // .Include(...)
            .Select(f => new UserInfoDto 
            {
                Id = f.SourceUser.Id,
                UserName = f.SourceUser.UserName,
                AvatarUrl = f.SourceUser.AvatarUrl,
                Email = f.SourceUser.Email
            })
            .ToListAsync();
    }
}