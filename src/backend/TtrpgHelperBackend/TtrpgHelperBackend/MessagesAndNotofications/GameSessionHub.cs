using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TtrpgHelperBackend.Services;

namespace TtrpgHelperBackend.MessagesAndNotofications;

[Authorize]
public class GameSessionHub : Hub
{
    private readonly ChatService _chatService;
    //private readonly ISessionService _sessionService;
    
    public GameSessionHub(ChatService chatService)
    {
        _chatService = chatService;
        // _sessionService = sessionService;
    }
    private string GetUserId()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            throw new HubException("Unauthorized: User ID claim (NameIdentifier) is missing.");
        return userId;
    }
    
    public async Task SendSessionMessage(string sessionId, string message)
    {
        var senderId =  GetUserId();
        
        await _chatService.SaveSessionMessage(int.Parse(senderId), int.Parse(sessionId), message);
        // Persist message
        await Clients.Group(sessionId).SendAsync("ReceiveSessionMessage", senderId, new
        {
            Message = message,
            TimeStamp = DateTime.Now,
            Type = "chat"
        });
    }

    // public async Task RollDice(string sessionId, string diceExpression) 
    // {
    //     var userId = GetUserId();
    //     // Logika rzutu 
    //     var rollResult = _sessionService.diceRoll(diceExpression);
    //
    //     // Wysyłamy wynik do wszystkich w sesji - nie tylko czat, ale np. animacja na ekranie
    //     await Clients.Group(sessionId).SendAsync("DiceRolled", new 
    //     {
    //         User = userId,
    //         Result = rollResult,
    //         Expression = diceExpression
    //     });
    // }
    
    public async Task JoinSession(string sessionId)
    {
        var userId = GetUserId();
        //bool hasAccess = await _sessionService.UserHasAccessToSession(userId, sessionId);
        if (false)
        {
            throw new HubException("Nie masz dostępu do tej sesji.");
        }
        
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        
        await Clients.Group(sessionId).SendAsync("UserJoined", userId);
    }

    public async Task LeaveSession(string sessionId)
    {
        var userId = GetUserId();
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
        await Clients.Group(sessionId).SendAsync("UserLeft", userId);
    }
}