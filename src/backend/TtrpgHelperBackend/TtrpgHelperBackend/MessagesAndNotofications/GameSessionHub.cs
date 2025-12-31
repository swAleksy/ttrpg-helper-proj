using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TtrpgHelperBackend.DTOs.Session;
using TtrpgHelperBackend.Models.Session;
using TtrpgHelperBackend.Services.Session;

namespace TtrpgHelperBackend.MessagesAndNotofications;

[Authorize]
public class GameSessionHub : Hub
{
    private readonly ISessionService _sessionService;
    private readonly ISessionEventService _sessionEventService;
    
    public GameSessionHub(ISessionService sessionService, ISessionEventService sessionEventService)
    {
        _sessionService = sessionService;
        _sessionEventService = sessionEventService;
    }
    
    // ==================
    // -- SESSION CHAT --
    public async Task SendSessionMessage(int sessionId, string message)
    {
        var userId =  GetUserId();
        if (!await _sessionService.CheckAccess(userId, sessionId)) throw new HubException("No access to this session");
        
        await BroadcastEvent(
            new CreateSessionEventDto
            {
                SessionId = sessionId,
                Type = SessionEventType.ChatMessage,
                DataJson = JsonSerializer.Serialize(new { message })
            },
            userId
        );
    }

    
    // ==========================
    // -- SESSION JOIN / LEAVE --
    public async Task JoinSession(int sessionId)
    {
        var userId = GetUserId();
        if (!await _sessionService.CheckAccess(userId, sessionId)) throw new HubException("No access to this session");
        
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId.ToString());
        
        await BroadcastEventNoDbSave(
            new CreateSessionEventDto
            {
                SessionId = sessionId,
                Type = SessionEventType.UserJoined,
                DataJson = JsonSerializer.Serialize(new
                {
                    userId = userId,
                    userName = Context.User?.FindFirstValue(ClaimTypes.Name)
                })
            },
            userId
        );
    }

    public async Task LeaveSession(int sessionId)
    {
        var userId = GetUserId();
        
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId.ToString());
        await BroadcastEventNoDbSave(
            new CreateSessionEventDto
            {
                SessionId = sessionId,
                Type = SessionEventType.UserLeft,
                DataJson = JsonSerializer.Serialize(new
                {
                    userId = userId,
                    userName = Context.User?.FindFirstValue(ClaimTypes.Name)
                })
            },
            userId
            );
    }
    
    
    // ====================
    // -- HELPER METHODS --
    private int GetUserId()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) throw new HubException("Unauthorized.");

        return int.Parse(userId);
    }

    private async Task BroadcastEvent(CreateSessionEventDto dto, int userId)
    {
        var savedEvent = await _sessionEventService.CreateEvent(dto, userId);
        if (savedEvent == null) throw new HubException("Could not create session event.");

        await Clients.Group(dto.SessionId.ToString()).SendAsync("SessionEventCreated", savedEvent);
    }
    
    private async Task BroadcastEventNoDbSave(CreateSessionEventDto dto, int userId)
    {
        var nonSavedEvent = await _sessionEventService.CreateEventWithoutSave(dto, userId);
        await Clients.Group(dto.SessionId.ToString()).SendAsync("SessionEventCreated", nonSavedEvent);
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userIdClaim = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return;

        var userId = int.Parse(userIdClaim);

        await base.OnDisconnectedAsync(exception);
    }
}
