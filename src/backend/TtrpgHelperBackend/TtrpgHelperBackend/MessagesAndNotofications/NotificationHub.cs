using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TtrpgHelperBackend.MessagesAndNotofications;

[Authorize]
public class NotificationHub : Hub
{
    private string GetUserId()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            throw new HubException("Unauthorized: User ID claim (NameIdentifier) is missing.");
        return userId;
    }
    
    public async Task SendNotification(string userId, string type, string message)
    {
        var senderId = GetUserId();

        await Clients.User(userId).SendAsync("ReceiveNotification", new {
            Type = type,
            Message = message,
            FromUser = senderId,
            Timestamp = DateTime.UtcNow
        });
    }
}