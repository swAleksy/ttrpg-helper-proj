using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.MessagesAndNotofications;
using TtrpgHelperBackend.Models;

namespace TtrpgHelperBackend.Services;

public interface INotificationService
{
    Task<Notification> SendNotificationAsync(string userId, NotificationType type, string title, string message, string? fromUserId = null);
    Task<List<Notification>> GetUnreadNotificationsAsync(string userId);
    Task MarkAsReadAsync(int notificationId, string userId);
}

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<NotificationHub> _hub;

    public NotificationService(ApplicationDbContext context, IHubContext<NotificationHub> hub)
    {
        _context = context;
        _hub = hub;
    }
    public async Task<Notification> SendNotificationAsync(string userId, NotificationType type, string title, string message, string? fromUserId = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            FromUserId = fromUserId
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        // Push real-time notification through SignalR
        await _hub.Clients.User(userId).SendAsync("ReceiveNotification", new
        {
            notification.Id,
            notification.Type,
            notification.Title,
            notification.Message,
            notification.FromUserId,
            notification.CreatedAt
        });

        return notification;
    }

    public async Task<List<Notification>> GetUnreadNotificationsAsync(string userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task MarkAsReadAsync(int notificationId, string userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

        if (notification == null)
            throw new Exception("Notification not found");

        notification.IsRead = true;
        await _context.SaveChangesAsync();
    }
}