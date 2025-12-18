using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs;
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

    public ChatService(ApplicationDbContext db, IHubContext<MainHub> hubContext)
    {
        _context = db;
        _hubContext = hubContext;
    }

// Dodano parametr senderName
    public async Task SendPrivateMessage(int senderId, string senderName, int receiverId, string content)
    {
        // 1. Walidacja (przeniesiona z Huba)
        // Opcjonalnie: Możesz pominąć to sprawdzenie dla wydajności, 
        // jeśli wierzysz, że frontend wysyła poprawne ID.
        // Ale dla spójności danych warto sprawdzić czy odbiorca istnieje.
        var receiverExists = await _context.Users.AnyAsync(u => u.Id == receiverId);
        if (!receiverExists)
            throw new Exception("Receiver not found");

        // 2. Zapis do Bazy
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
        
        // 3. Przygotowanie DTO
        // Nie musimy już robić zapytania do bazy po SenderName, bo dostaliśmy je w argumencie!
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

        // 4. Wysyłka SignalR
        // Upewniamy się, że konwertujemy ID na string, bo SignalR User ID to string
        await _hubContext.Clients.User(receiverId.ToString()).SendAsync("ReceivePrivateMessage", msgDto);
        await _hubContext.Clients.User(senderId.ToString()).SendAsync("ReceivePrivateMessage", msgDto);
    }
    
    public async Task<List<MessageDto>> GetPrivateChatHistory(int userId, int otherUserId)
    {
        return await _context.ChatMessages
            .AsNoTracking() // Ważne dla wydajności przy samym odczycie!
            .Include(m => m.Sender) // Ładujemy dane nadawcy
            .Where(m =>
                (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                (m.SenderId == otherUserId && m.ReceiverId == userId))
            .OrderBy(m => m.SentAt)
            .Select(m => new MessageDto // Projekcja bezpośrednio do DTO
            {
                Id = m.Id,
                SenderId = m.SenderId,
                SenderName = m.Sender.UserName, // Dostępne dzięki .Include
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
                SenderName = m.Sender.UserName, // Wymaga Include(m => m.Sender) lub User w modelu
                ReceiverId = m.ReceiverId.Value,
                Content = m.Content,
                SentAt = m.SentAt,
                IsRead = m.IsRead
            })
            .ToListAsync();
    }
}