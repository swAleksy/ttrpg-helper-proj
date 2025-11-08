

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs.Session;
using TtrpgHelperBackend.Services.Session;

namespace TtrpgHelperBackend.Controllers.Session;

[ApiController]
[Route("api/[controller]")]
public class SessionController : ControllerBase
{
        private readonly ISessionService _sessionService;
        private readonly ApplicationDbContext _context;

        public SessionController(ISessionService sessionService, ApplicationDbContext context)
        {
                _sessionService = sessionService;
                _context = context;
        }
        
        // GET: api/session/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<GetSessionDto>> Get(int id)
        {
            var session = await _sessionService.GetOneSession(id);

            if (session == null) return NotFound();

            return Ok(session);
        }
        
        // GET: api/session
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SummarizeSessionDto>>> GetAll()
        {
            var sessions = await _sessionService.GetAllSessions();
            
            return Ok(sessions);
        }
        
        // POST: api/session
        [HttpPost]
        public async Task<ActionResult<GetSessionDto>> Create([FromBody] CreateSessionDto dto)
        {
            var created = await _sessionService.CreateSession(dto);
            
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
        
        // PUT: api/session/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<GetSessionDto>> Update(int id, [FromBody] UpdateSessionDto dto)
        {
            if (id != dto.Id) return BadRequest("ID in URL does not match ID in body");
            
            var updated = await _sessionService.UpdateSession(dto);
            
            if (updated == null) return NotFound();
            
            return Ok(updated);
        }
        
        // DELETE: api/session/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _sessionService.DeleteSession(id);
            
            if (!deleted) return NotFound();
            
            return NoContent();
        }
        
        // POST: api/session/{sessionId}/players/{playerId}
        [Authorize]
        [HttpPost("{sessionId}/players/{playerId}")]
        public async Task<ActionResult<GetSessionDto>> AddPlayer(int sessionId, int playerId)
        {
            var updatedSession = await _sessionService.AddPlayer(sessionId, playerId);
            
            if (updatedSession == null) return NotFound();
            
            return Ok(updatedSession);
        }
}