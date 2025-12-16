using Microsoft.AspNetCore.Authorization;

using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.Services;

namespace TtrpgHelperBackend.MessagesAndNotofications;

[Authorize]
public class MainHub : Hub
{
    // private static readonly Dictionary<string, string> _connections = new();
    private readonly IChatService _chatService;
    private readonly INotificationService _notificationService;

    public MainHub(IChatService chatService, INotificationService notificationService)
    {
        _chatService = chatService;
        _notificationService = notificationService;
    }

    public async Task SendPrivateMessage(int receiverId, string message)
    {
        var senderIdStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        // Pobieramy nazwę usera z tokena, żeby nie robić SELECT w bazie
        var senderName = Context.User?.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(senderIdStr) || string.IsNullOrEmpty(senderName))
            throw new HubException("Unauthorized");

        int senderId = int.Parse(senderIdStr);

        if (senderId == receiverId)
            throw new HubException("Cannot send message to yourself");

        try
        {
            // Przekazujemy senderName do serwisu
            await _chatService.SavePrivateMessage(senderId, senderName, receiverId, message);
        }
        catch (Exception ex)
        {
            // Łapiemy wyjątek z serwisu (np. "Receiver not found") i rzucamy HubException
            throw new HubException(ex.Message);
        }
    }
    // public override async Task OnConnectedAsync()
    // {
    //     var userId =  GetUserId();
    //     _connections[userId] = Context.ConnectionId;
    //     await base.OnConnectedAsync();
    // }
    //
    // public override async Task OnDisconnectedAsync(Exception? exception)
    // {
    //     var userId =  GetUserId();
    //     _connections.Remove(userId);
    //     await base.OnDisconnectedAsync(exception);
    // }
    
    //
    // public async Task SendNotification(string userId, string type, string message)
    // {
    //     var senderId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    //     
    //     if (string.IsNullOrEmpty(senderId)) 
    //         throw new HubException("Unauthorized");
    //
    //     await _notificationService.SendNotificationAsync();
    // }

}