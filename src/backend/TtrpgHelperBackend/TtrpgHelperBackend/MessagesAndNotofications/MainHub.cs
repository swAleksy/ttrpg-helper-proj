using Microsoft.AspNetCore.Authorization;

using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using TtrpgHelperBackend.Services;

namespace TtrpgHelperBackend.MessagesAndNotofications;

[Authorize]
public class MainHub : Hub
{
    // private static readonly Dictionary<string, string> _connections = new();
    private readonly ChatService _chatService;

    public MainHub(ChatService chatService)
    {
        _chatService = chatService;
    }
    
    private string GetUserId()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            throw new HubException("Unauthorized: User ID missing in token.");

        return userId;
    }
    
    public override async Task OnConnectedAsync()
    {
        //var userId =  GetUserId();
        //_connections[userId] = Context.ConnectionId;
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        //var userId =  GetUserId();
        //_connections.Remove(userId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendPrivateMessage(string receiverId, string message)
    {
        var senderId =  GetUserId();
        
        // Save to DB via a service or repository
        await _chatService.SavePrivateMessage(senderId, receiverId, message);

        await Clients.User(receiverId).SendAsync("ReceivePrivateMessage", senderId, message);
        await Clients.User(senderId).SendAsync("ReceivePrivateMessage", senderId, message); // echo to sender

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