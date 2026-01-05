using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs;
using TtrpgHelperBackend.Helpers;
using TtrpgHelperBackend.MessagesAndNotofications;
using TtrpgHelperBackend.Models;
namespace TtrpgHelperBackend.Services;

public interface IChatService
{
    Task SendPrivateMessage(int senderId, string senderName, int receiverId, string content);
    Task SaveSessionMessage(int senderId, int sessionId, string content);
    Task<List<MessageDto>> GetPrivateChatHistory(int userId, int otherUserId);
    Task<List<MessageDto>> GetSessionChatHistory(int sessionId);
}

public class ChatService :  IChatService
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<MainHub> _hubContext;
    private readonly INotificationService _notificationService;
    private readonly UserHelper _userHelper;

    public ChatService(ApplicationDbContext db, IHubContext<MainHub> hubContext, INotificationService notificationService, UserHelper userHelper)
    {
        _context = db;
        _hubContext = hubContext;
        _notificationService = notificationService;
        _userHelper = userHelper;
    }

    public async Task SendPrivateMessage(int senderId, string senderName, int receiverId, string content)
    {
        var receiverExists = await _context.Users.AnyAsync(u => u.Id == receiverId);
        if (!receiverExists)
            throw new Exception("Receiver not found");

        var msg = new ChatMessage
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = content,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        _context.ChatMessages.Add(msg);
        await _context.SaveChangesAsync();
        
        var msgDto = new MessageDto
        {
            Id = msg.Id,
            SenderId = senderId,
            SenderName = senderName, 
            ReceiverId = receiverId,
            Content = content,
            SentAt = msg.SentAt,
            IsRead = msg.IsRead
        };
        
        // Wysyłka signalr dla obu stron
        await _hubContext.Clients.User(receiverId.ToString()).SendAsync("ReceivePrivateMessage", msgDto);
        await _hubContext.Clients.User(senderId.ToString()).SendAsync("ReceivePrivateMessage", msgDto);
        await _notificationService.SendNotification(
            receiverId, 
            NotificationType.NewMessage, 
            "Nowa wiadomosc", 
            $"Użytkownik {_userHelper.GetUserName()} wysłał Ci wiadomosc.", 
            senderId
        );
    }
    
    public async Task<List<MessageDto>> GetPrivateChatHistory(int userId, int otherUserId)
    {
        return await _context.ChatMessages
            .AsNoTracking() 
            .Include(m => m.Sender) 
            .Where(m =>
                (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                (m.SenderId == otherUserId && m.ReceiverId == userId))
            .OrderBy(m => m.SentAt)
            .Select(m => new MessageDto 
            {
                Id = m.Id,
                SenderId = m.SenderId,
                SenderName = m.Sender.UserName,
                ReceiverId = m.ReceiverId.Value,
                Content = m.Content,
                SentAt = m.SentAt,
                IsRead = m.IsRead
            })
            .ToListAsync();
    }

    // Session DO POPRAWY 
    public async Task SaveSessionMessage(int senderId, int sessionId, string content)
    {
        var msg = new ChatMessage
        {
            SenderId = senderId,
            SessionId = sessionId.ToString(),
            Content = content
        };
        _context.ChatMessages.Add(msg);
        await _context.SaveChangesAsync();
    }
    public async Task<List<MessageDto>> GetSessionChatHistory(int sessionId)
    {
        return await _context.ChatMessages
            .AsNoTracking()
            .Where(m => m.SessionId == sessionId.ToString()) 
            .OrderBy(m => m.SentAt)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                SenderName = m.Sender.UserName, 
                ReceiverId = m.ReceiverId.Value,
                Content = m.Content,
                SentAt = m.SentAt,
                IsRead = m.IsRead
            })
            .ToListAsync();
    }
}