using Microsoft.AspNetCore.Authorization;

using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using TtrpgHelperBackend.Services;

namespace TtrpgHelperBackend.MessagesAndNotofications;

[Authorize]
public class ChatHub : Hub
{
    // private static readonly Dictionary<string, string> _connections = new();
    private readonly ChatService _chatService;

    public ChatHub(ChatService chatService)
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
        
        await _chatService.SavePrivateMessage(senderId, receiverId, message);

        // Save to DB via a service or repository
        await Clients.User(receiverId).SendAsync("ReceivePrivateMessage", senderId, message);
        await Clients.User(senderId).SendAsync("ReceivePrivateMessage", senderId, message); // echo to sender

    }

    public async Task SendTeamMessage(string sessionId, string message)
    {
        var senderId =  GetUserId();
        
        await _chatService.SaveTeamMessage(senderId, sessionId, message);
        // Persist message
        await Clients.Group(sessionId).SendAsync("ReceiveTeamMessage", senderId, message);
    }

    public async Task JoinTeam(string sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
    }

    public async Task LeaveTeam(string sessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
    }
    

}