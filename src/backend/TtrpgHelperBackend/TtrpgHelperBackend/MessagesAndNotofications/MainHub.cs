using Microsoft.AspNetCore.Authorization;

using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.Models;
using TtrpgHelperBackend.Services;

namespace TtrpgHelperBackend.MessagesAndNotofications;

[Authorize]
public class MainHub : Hub
{
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
        var senderName = Context.User?.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(senderIdStr) || string.IsNullOrEmpty(senderName))
            throw new HubException("Unauthorized");

        int senderId = int.Parse(senderIdStr);

        if (senderId == receiverId)
            throw new HubException("Cannot send message to yourself");

        try
        {
            await _chatService.SendPrivateMessage(senderId, senderName, receiverId, message);
        }
        catch (Exception ex)
        {
            throw new HubException(ex.Message);
        }
    }
    public async Task SendNotification(int receiverId, NotificationType type, string message, string? title)
    {
        var senderIdStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var senderName = Context.User?.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(senderIdStr) || string.IsNullOrEmpty(senderName))
            throw new HubException("Unauthorized");

        int senderId = int.Parse(senderIdStr);
        
        try
        {
            await _notificationService.SendNotification(receiverId, type, title, message, senderId);
        }
        catch (Exception ex)
        {
            throw new HubException(ex.Message);
        }
    }
    
    // public override async Task OnConnectedAsync() {}
    // public override async Task OnDisconnectedAsync(Exception? exception) {}
    
}