using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.Models;
namespace TtrpgHelperBackend.Services;

public interface IChatService
{
    Task SavePrivateMessage(string senderId, string receiverId, string content);
    Task SaveSessionMessage(string senderId, string sessionId, string content);
    Task<List<ChatMessage>> GetPrivateChatHistory(string userId, string otherUserId);
    Task<List<ChatMessage>> GetSessionChatHistory(string sessionId);
}

public class ChatService :  IChatService
{
    private readonly ApplicationDbContext _context;

    public ChatService(ApplicationDbContext db)
    {
        _context = db;
    }

    public async Task SavePrivateMessage(string senderId, string receiverId, string content)
    {
        var msg = new ChatMessage
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = content
        };
        _context.ChatMessages.Add(msg);
        await _context.SaveChangesAsync();
    }

    public async Task SaveSessionMessage(string senderId, string sessionId, string content)
    {
        var msg = new ChatMessage
        {
            SenderId = senderId,
            SessionId = sessionId,
            Content = content
        };
        _context.ChatMessages.Add(msg);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ChatMessage>> GetPrivateChatHistory(string userId, string otherUserId)
    {
        return await _context.ChatMessages
            .Where(m => (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                        (m.SenderId == otherUserId && m.ReceiverId == userId))
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }

    public async Task<List<ChatMessage>> GetSessionChatHistory(string sessionId)
    {
        return await _context.ChatMessages
            .Where(m => m.SessionId == sessionId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }    
}