using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs.Session;
using TtrpgHelperBackend.Helpers;
using TtrpgHelperBackend.Services.Session;

namespace TtrpgHelperBackend.Controllers.Session;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SessionEventController : ControllerBase
{
    private readonly ISessionEventService _sessionEventService;
    private readonly UserHelper _userHelper;

    public SessionEventController(ISessionEventService sessionEventService, UserHelper userHelper)
    {
        _sessionEventService = sessionEventService;
        _userHelper = userHelper;
    }

    [HttpPost]
    public async Task<ActionResult<GetSessionEventDto>> CreateSessionEvent([FromBody] CreateSessionEventDto dto)
    {
        var userId = _userHelper.GetUserId();
        if (userId == null) return Unauthorized("User ID not found in token.");

        var result = await _sessionEventService.CreateEvent(dto, userId.Value);
        if (result == null) return NotFound("Session not found.");

        return Ok(result);
    }
    
    [HttpGet("{sessionId}")]
    public async Task<ActionResult<IEnumerable<GetSessionEventDto>>> GetEvents(int sessionId)
    {
        var userId = _userHelper.GetUserId();
        if (userId == null) return Unauthorized("User ID not found in token.");

        var events = await _sessionEventService.GetEventsForSession(sessionId);
        
        return Ok(events);
    }
}
