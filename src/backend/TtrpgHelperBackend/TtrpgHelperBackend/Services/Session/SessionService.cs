using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs.Session;
using TtrpgHelperBackend.Models.Session;
using SessionModel = TtrpgHelperBackend.Models.Session.Session;

namespace TtrpgHelperBackend.Services.Session;

public interface ISessionService
{
    Task<GetSessionDto> GetOneSession(int id);
    Task<IEnumerable<SummarizeSessionDto>> GetAllSessions();
    Task<GetSessionDto> CreateSession(CreateSessionDto dto);
    Task<GetSessionDto> UpdateSession(UpdateSessionDto dto);
    Task<bool> DeleteSession(int id);
    Task<GetSessionDto> AddPlayer(int sessionId, int playerId);
}

public class SessionService : ISessionService
{
    private readonly ApplicationDbContext _db;

    public SessionService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<GetSessionDto> GetOneSession(int id)
    {
        var session = await _db.Sessions
            .Include(s => s.GameMaster)
            .Include(s => s.Players)
                .ThenInclude(sp => sp.Player)
            .FirstOrDefaultAsync(s => s.Id == id);
        
        if (session == null) return null;
        
        return MapToGetSessionDto(session);
    }

    public async Task<IEnumerable<SummarizeSessionDto>> GetAllSessions()
    {
        var sessions = await _db.Sessions.ToListAsync();

        return sessions.Select(s => new SummarizeSessionDto
        {
            Id = s.Id,
            Name = s.Name,
            Status = s.Status,
            ScheduledDate = s.ScheduledDate,
        });
    }

    public async Task<GetSessionDto> CreateSession(CreateSessionDto dto)
    {
        var session = new  SessionModel
        {
            Name = dto.Name,
            Description = dto.Description,
            ScheduledDate = dto.ScheduledDate,
            Status = dto.Status,
            GameMasterId = dto.GameMasterId,
        };
        
        _db.Sessions.Add(session);
        await _db.SaveChangesAsync();
        
        await _db.Entry(session).Reference(s => s.GameMaster).LoadAsync();
        
        return MapToGetSessionDto(session);
    }
    
    public async Task<GetSessionDto> UpdateSession(UpdateSessionDto dto)
    {
        var session = await _db.Sessions.FindAsync(dto.Id);
        
        if (session == null) return null!;

        session.Name = dto.Name;
        session.Description = dto.Description;
        session.ScheduledDate = dto.ScheduledDate;
        session.Status = dto.Status;

        await _db.SaveChangesAsync();

        await _db.Entry(session).Reference(s => s.GameMaster).LoadAsync();
        await _db.Entry(session).Collection(s => s.Players).Query().Include(sp => sp.Player).LoadAsync();

        return MapToGetSessionDto(session);
    }

    public async Task<bool> DeleteSession(int id)
    {
        var session = await _db.Sessions.FindAsync(id);
        
        if (session == null) return false;

        _db.Sessions.Remove(session);
        await _db.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<GetSessionDto> AddPlayer(int sessionId, int playerId)
    {
        var session = await _db.Sessions
            .Include(s => s.Players)
            .ThenInclude(sp => sp.Player)
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session == null) return null!;
        
        if (session.Players.Any(p => p.PlayerId == playerId)) return MapToGetSessionDto(session);

        var sessionPlayer = new SessionPlayer
        {
            SessionId = sessionId,
            PlayerId = playerId,
        };

        _db.SessionPlayers.Add(sessionPlayer);
        await _db.SaveChangesAsync();

        await _db.Entry(session).Collection(s => s.Players).Query().Include(sp => sp.Player).LoadAsync();

        return MapToGetSessionDto(session);
    }

    private GetSessionDto MapToGetSessionDto(SessionModel session)
    {
        return new GetSessionDto
        {
            Id = session.Id,
            Name = session.Name,
            Description = session.Description,
            ScheduledDate = session.ScheduledDate,
            Status = session.Status,
            GameMasterId = session.GameMasterId,
            GameMasterName = session.GameMaster.UserName,
            Players = session.Players.Select(sp => new SessionPlayerDto
            {
                PlayerId = sp.PlayerId,
                PlayerName = sp.Player.UserName
            }).ToList()
        };
    }
}