using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs.Session;
using TtrpgHelperBackend.Services.Session;

namespace TtrpgHelperBackend.Controllers.Session;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SessionController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public SessionController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }
        
    // GET one specified session of GM
    [HttpGet("{id}")]
    public async Task<ActionResult<GetSessionDto>> GetSession(int id)
    {
        var session = await _sessionService.GetSession(id);
        if (session == null) return NotFound();
    
        return Ok(session);
    }
        
    // GET all sessions of GM
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetSessionDto>>> GetSessions()
    {
        var sessions = await _sessionService.GetSessions();
            
        return Ok(sessions);
    }
        
    // POST
    [HttpPost("create")]
    public async Task<ActionResult<GetSessionDto>> CreateSession([FromBody] CreateSessionDto dto)
    {
        var newSession = await _sessionService.CreateSession(dto);
        
        return Ok(newSession);
    }
        
    // PUT
    [HttpPut("update/{id}")]
    public async Task<ActionResult<GetSessionDto>> UpdateSession(int id, [FromBody] UpdateSessionDto dto)
    {
        if (id != dto.Id) return BadRequest();
        
        var updatedSession = await _sessionService.UpdateSession(dto);
        if (updatedSession == null) return NotFound("Session not found.");
        
        return Ok(updatedSession);
    }
        
    // DELETE
    [HttpDelete("delete/{id}")]
    public async Task<ActionResult> DeleteSession(int id)
    {
        var deleted = await _sessionService.DeleteSession(id);
        if (!deleted) return NotFound("Session not found");
        
        return Ok("Character deleted successfully.");
    }
    
    // POST
    [HttpPost("addPlayer/{sessionId}/{playerId}")]
    public async Task<ActionResult<GetSessionDto>> AddPlayer(int sessionId, int playerId)
    {
        var updatedSession = await _sessionService.AddPlayer(sessionId, playerId);
        if (updatedSession == null) return NotFound("Session not found.");
        
        return Ok(updatedSession);
    }
}
