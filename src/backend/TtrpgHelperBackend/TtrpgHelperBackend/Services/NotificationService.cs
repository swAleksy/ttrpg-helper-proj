using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs;
using TtrpgHelperBackend.MessagesAndNotofications;
using TtrpgHelperBackend.Models;

namespace TtrpgHelperBackend.Services;

public interface INotificationService
{
    Task<NotificationDto> SendNotificationAsync(int targetUserId, NotificationType type, string title, string message,
        int? fromUserId = null);
    Task<List<NotificationDto>> GetUnreadNotificationsAsync(int userId);
    Task MarkAsReadAsync(int notificationId, int userId);
}

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<MainHub> _hubContext;

    public NotificationService(ApplicationDbContext context, IHubContext<MainHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task<NotificationDto> SendNotificationAsync(int targetUserId, NotificationType type, string title, string message, int? fromUserId = null)
    {
        // 1. Tworzymy encję i zapisujemy w bazie (trwałość danych)
        var notification = new Notification
        {
            UserId = targetUserId,
            Type = type,
            Title = title,
            Message = message,
            FromUserId = fromUserId,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        // 2. Mapujemy na DTO (żeby nie wysyłać całej encji z cyklami)
        var dto = new NotificationDto
        {
            Id = notification.Id,
            Type = notification.Type.ToString(), // np. "FriendRequest"
            Title = notification.Title,
            Message = notification.Message,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt
        };

        // 3. Wysyłamy przez SignalR
        // Uwaga: Wysyłamy DWA zdarzenia dla wygody frontendu:
        
        // A. Ogólne powiadomienie (do listy powiadomień)
        await _hubContext.Clients.User(targetUserId.ToString())
            .SendAsync("ReceiveNotification", dto);

        // B. Specyficzne zdarzenie (jeśli chcesz np. podbić licznik znajomych w czasie rzeczywistym)
        if (type == NotificationType.NewMessage)
        {
             // Opcjonalnie: ChatService już to robi, więc tu można pominąć
        }
        else if (type == NotificationType.AddedToGroup) // Przykład: FriendRequest traktujemy jako osobny typ
        {
             // Możesz wysłać specyficzny event, jeśli frontend tego wymaga
             await _hubContext.Clients.User(targetUserId.ToString())
                 .SendAsync("ReceiveFriendRequest");
        }

        return dto;
    }

    public async Task<List<NotificationDto>> GetUnreadNotificationsAsync(int userId)
    {
        return await _context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type.ToString(),
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            })
            .ToListAsync();
    }

    public async Task MarkAsReadAsync(int notificationId, int userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

        if (notification != null)
        {
            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }
}