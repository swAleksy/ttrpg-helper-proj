using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs.Session;
using TtrpgHelperBackend.Models.Session;

namespace TtrpgHelperBackend.Services.Session;

public interface ISessionEventService
{
    Task<GetSessionEventDto?> CreateEvent(CreateSessionEventDto dto, int userId);
    Task<IEnumerable<GetSessionEventDto>> GetEventsForSession(int sessionId);
}

public class SessionEventService : ISessionEventService
{
    private readonly ApplicationDbContext _db;

    public SessionEventService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<GetSessionEventDto?> CreateEvent(CreateSessionEventDto dto, int userId)
    {
        if (!await _db.Sessions.AnyAsync(s => s.Id == dto.SessionId)) return null;
        
        var sessionEvent = new SessionEvent
        {
            SessionId = dto.SessionId,
            UserId = userId,
            Type = dto.Type,
            DataJson = dto.DataJson,
            Timestamp = DateTime.Now
        };
        
        _db.SessionEvents.Add(sessionEvent);
        await _db.SaveChangesAsync();

        sessionEvent = await _db.SessionEvents
            .Include(e => e.User)
            .FirstAsync(e => e.Id == sessionEvent.Id);
        
        return Dto(sessionEvent);
    }

    public async Task<IEnumerable<GetSessionEventDto>> GetEventsForSession(int sessionId)
    {
        var sessionEvents = await _db.SessionEvents
            .Where(e => e.SessionId == sessionId)
            .Include(e => e.User)
            .OrderBy(e => e.Timestamp)
            .ThenBy(e => e.Id)
            .ToListAsync();

        return sessionEvents.Select(Dto);
    }
    
    private static GetSessionEventDto Dto(SessionEvent e)
    {
        return new GetSessionEventDto
        {
            Id = e.Id,
            SessionId = e.SessionId,
            UserId = e.UserId,
            UserName = e.User.UserName,
            Type = e.Type,
            DataJson = e.DataJson,
            Timestamp = e.Timestamp
        };
    }
}
