using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.Services.Session;

namespace TtrpgHelperBackend.Controllers.Session;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminSessionController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public AdminSessionController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var session = await _sessionService.GetSessionForAdmin(id);
        if (session == null) return NotFound("Session not found.");

        return Ok(session);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status)
    {
        var sessions = await _sessionService.GetSessionsForAdmin(status);
        
        return Ok(sessions);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _sessionService.DeleteSessionForAdmin(id);
        if (!deleted) return NotFound("Session not found.");

        return Ok("Session deleted successfully.");
    }
}
