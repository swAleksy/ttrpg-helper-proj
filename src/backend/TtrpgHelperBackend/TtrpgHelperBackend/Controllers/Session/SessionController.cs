using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs.Session;
using TtrpgHelperBackend.Services.Session;
using TtrpgHelperBackend.Helpers;

namespace TtrpgHelperBackend.Controllers.Session;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SessionController : ControllerBase
{
    private readonly ISessionService _sessionService;
    private readonly UserHelper _userHelper;

    public SessionController(ISessionService sessionService, UserHelper userHelper)
    {
        _sessionService = sessionService;
        _userHelper = userHelper;
    }

    // -- GAMEMASTER --
    // GET one specified session of GM
    [HttpGet("gm/{id}")]
    public async Task<ActionResult<GetSessionDto>> GetSessionForGm(int id)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var session = await _sessionService.GetSessionForGm(id, gameMasterId.Value);
        if (session == null) return NotFound("Session not found.");

        return Ok(session);
    }

    // GET all sessions of GM, optional status filtering
    [HttpGet("gm")]
    public async Task<ActionResult<IEnumerable<GetSessionDto>>> GetSessionsForGm([FromQuery] string? status = null)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var sessions = await _sessionService.GetSessionsForGm(gameMasterId.Value, status);
        
        return Ok(sessions);
    }
    // -- GAMEMASTER --
    
    // -- PLAYER --
    // GET one specified session of PLAYER
    [HttpGet("player/{id}")]
    public async Task<ActionResult<GetSessionDto>> GetSessionForPlayer(int id)
    {
        var playerId = _userHelper.GetUserId();
        if (playerId == null) return Unauthorized("User ID not found in token.");

        var session = await _sessionService.GetSessionForPlayer(id, playerId.Value);
        if (session == null)
            return NotFound("Session not found or you are not a participant.");

        return Ok(session);
    }

    // GET all sessions of PLAYER, optional status filtering
    [HttpGet("player")]
    public async Task<ActionResult<IEnumerable<GetSessionDto>>> GetSessionsForPlayer([FromQuery] string? status = null)
    {
        var playerId = _userHelper.GetUserId();
        if (playerId == null) return Unauthorized("User ID not found in token.");

        var sessions = await _sessionService.GetSessionsForPlayer(playerId.Value, status);
       
        return Ok(sessions);
    }
    // -- PLAYER --
    
    [HttpGet("both/{id}")]
    public async Task<ActionResult<GetSessionDto>> GetSessionForPlayerOrGm(int id)
    {
        var playerId = _userHelper.GetUserId();
        if (playerId == null) return Unauthorized("User ID not found in token.");

        var session = await _sessionService.GetSession(id, playerId.Value);
        if (session == null)
            return NotFound("Session not found or you are not a participant.");

        return Ok(session);
    }
    
    // CREATE (GM only)
    [HttpPost("gm/create")]
    public async Task<ActionResult<GetSessionDto?>> CreateSession([FromBody] CreateSessionDto dto)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var newSession = await _sessionService.CreateSession(dto, gameMasterId.Value);
        if (newSession == null) return BadRequest("You cannot create a session in a campaign you do not own.");
        
        var sessionId = newSession.Id;
        var updated = await _sessionService.AddPlayer(sessionId, gameMasterId.Value, gameMasterId.Value);
        if (updated == null) return NotFound("Could not add admin to session.");
        
        return Ok(newSession);
    }

    // UPDATE (GM only)
    [HttpPut("gm/update/{id}")]
    public async Task<ActionResult<GetSessionDto>> UpdateSession(int id, [FromBody] UpdateSessionDto dto)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        if (id != dto.Id) return BadRequest("ID mismatch.");

        var updated = await _sessionService.UpdateSession(dto, gameMasterId.Value);
        if (updated == null) return NotFound("Session not found or not owned by GM.");

        return Ok(updated);
    }
    
    // ARCHIVE (GM only)
    [HttpPut("gm/archive/{id}")]
    public async Task<ActionResult> ArchiveSession(int id)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");
        
        var archived = await _sessionService.ArchiveSession(id, gameMasterId.Value);
        if (!archived) return NotFound("Session not found or not owned by GM.");
        
        return Ok("Session successfully archived.");
    }

    // DELETE (GM only)
    [HttpDelete("gm/delete/{id}")]
    public async Task<ActionResult> DeleteSession(int id)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var deleted = await _sessionService.DeleteSession(id, gameMasterId.Value);
        if (!deleted) return NotFound("Session not found or not owned by GM.");

        return Ok("Session deleted successfully.");
    }

    // ADD PLAYER (GM only)
    [HttpPost("gm/addPlayer/{sessionId}/{playerId}")]
    public async Task<ActionResult<GetSessionDto>> AddPlayer(int sessionId, int playerId)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");
        if (gameMasterId == playerId) return Unauthorized("Game master cannot be added as a player.");

        var updated = await _sessionService.AddPlayer(sessionId, gameMasterId.Value, playerId);
        if (updated == null) return NotFound("Session not found or not owned by GM.");

        return Ok(updated);
    }
}
