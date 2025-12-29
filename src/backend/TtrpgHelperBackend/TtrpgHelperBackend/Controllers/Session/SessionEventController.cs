using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TtrpgHelperBackend.DTOs.Session;
using TtrpgHelperBackend.Helpers;
using TtrpgHelperBackend.MessagesAndNotofications;
using TtrpgHelperBackend.Services.Session;

namespace TtrpgHelperBackend.Controllers.Session;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SessionEventController : ControllerBase
{
    private readonly ISessionService _sessionService;
    private readonly ISessionEventService _sessionEventService;
    private readonly UserHelper _userHelper;
    private readonly IHubContext<GameSessionHub> _hubContext;

    public SessionEventController(ISessionService sessionService, ISessionEventService sessionEventService, UserHelper userHelper, IHubContext<GameSessionHub> hubContext)
    {
        _sessionService = sessionService;
        _sessionEventService = sessionEventService;
        _userHelper = userHelper;
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<ActionResult<GetSessionEventDto>> CreateSessionEvent([FromBody] CreateSessionEventDto dto)
    {
        var userId = _userHelper.GetUserId();
        if (userId == null) return Unauthorized("User ID not found in token.");

        if (!await _sessionService.CheckAccess(userId.Value, dto.SessionId)) return Forbid("No access to this session.");
        
        var result = await _sessionEventService.CreateEvent(dto, userId.Value);
        if (result == null) return NotFound("Session not found.");
        
        await _hubContext.Clients.Group(dto.SessionId.ToString()).SendAsync("SessionEventCreated", result);

        return Ok(result);
    }
    
    [HttpGet("{sessionId}")]
    public async Task<ActionResult<IEnumerable<GetSessionEventDto>>> GetEvents(int sessionId)
    {
        var userId = _userHelper.GetUserId();
        if (userId == null) return Unauthorized("User ID not found in token.");

        if (!await _sessionService.CheckAccess(userId.Value, sessionId)) return Forbid("No access to this session.");
        
        var events = await _sessionEventService.GetEventsForSession(sessionId);
        
        return Ok(events);
    }
}
