using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs.Session;
using TtrpgHelperBackend.Models.Session;
using SessionModel = TtrpgHelperBackend.Models.Session.Session;

namespace TtrpgHelperBackend.Services.Session;

public interface ISessionService
{
    Task<GetSessionDto?> GetSession(int id);
    Task<IEnumerable<GetSessionDto>> GetSessions();
    Task<GetSessionDto> CreateSession(CreateSessionDto dto);
    Task<GetSessionDto?> UpdateSession(UpdateSessionDto dto);
    Task<bool> DeleteSession(int id);
    Task<GetSessionDto?> AddPlayer(int sessionId, int playerId);
}

public class SessionService : ISessionService
{
    private readonly ApplicationDbContext _db;

    public SessionService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<GetSessionDto?> GetSession(int id)
    {
        var session = await _db.Sessions
            .Include(s => s.GameMaster)
            .Include(s => s.Players).ThenInclude(sp => sp.Player)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (session == null) return null;

        return new GetSessionDto
        {
            Id = session.Id,
            Name = session.Name,
            Description = session.Description,
            ScheduledDate = session.ScheduledDate,
            Status = session.Status,
            GameMasterId = session.GameMaster.Id,
            GameMasterName = session.GameMaster.UserName,
            Players = session.Players.Select(p => new SessionPlayerDto
            {
                PlayerId = p.Player.Id,
                PlayerName = p.Player.UserName
            }).ToList()
        };
    }

    public async Task<IEnumerable<GetSessionDto>> GetSessions()
    {
        var sessions = await _db.Sessions
            .Include(s => s.GameMaster)
            .Include(s => s.Players).ThenInclude(sp => sp.Player)
            .ToListAsync();

        return sessions.Select(session => new GetSessionDto
        {
            Id = session.Id,
            Name = session.Name,
            Description = session.Description,
            ScheduledDate = session.ScheduledDate,
            Status = session.Status,
            GameMasterId = session.GameMaster.Id,
            GameMasterName = session.GameMaster.UserName,
            Players = session.Players.Select(p => new SessionPlayerDto
            {
                PlayerId = p.Player.Id,
                PlayerName = p.Player.UserName
            }).ToList()
        });
    }

    public async Task<GetSessionDto> CreateSession(CreateSessionDto dto)
    {
        var session = new SessionModel
        {
            Name = dto.Name,
            Description = dto.Description,
            ScheduledDate = dto.ScheduledDate,
            Status = dto.Status,
            GameMasterId = dto.GameMasterId,
            CampaignId = dto.CampaignId
        };

        _db.Sessions.Add(session);
        await _db.SaveChangesAsync();

        await _db.Entry(session).Reference(s => s.GameMaster).LoadAsync();

        return new GetSessionDto
        {
            Id = session.Id,
            Name = session.Name,
            Description = session.Description,
            ScheduledDate = session.ScheduledDate,
            Status = session.Status,
            GameMasterId = session.GameMaster.Id,
            GameMasterName = session.GameMaster.UserName,
            Players = new List<SessionPlayerDto>()
        };
    }

    public async Task<GetSessionDto?> UpdateSession(UpdateSessionDto dto)
    {
        var session = await _db.Sessions
            .Include(s => s.Players).ThenInclude(sp => sp.Player)
            .Include(s => s.GameMaster)
            .FirstOrDefaultAsync(s => s.Id == dto.Id);

        if (session == null)
            return null;

        session.Name = dto.Name;
        session.Description = dto.Description;
        session.ScheduledDate = dto.ScheduledDate;
        session.Status = dto.Status;

        await _db.SaveChangesAsync();

        return new GetSessionDto
        {
            Id = session.Id,
            Name = session.Name,
            Description = session.Description,
            ScheduledDate = session.ScheduledDate,
            Status = session.Status,
            GameMasterId = session.GameMaster.Id,
            GameMasterName = session.GameMaster.UserName,
            Players = session.Players.Select(p => new SessionPlayerDto
            {
                PlayerId = p.Player.Id,
                PlayerName = p.Player.UserName
            }).ToList()
        };
    }

    public async Task<bool> DeleteSession(int id)
    {
        var session = await _db.Sessions.FindAsync(id);

        if (session == null)
            return false;

        _db.Sessions.Remove(session);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<GetSessionDto?> AddPlayer(int sessionId, int playerId)
    {
        var session = await _db.Sessions
            .Include(s => s.Players).ThenInclude(sp => sp.Player)
            .Include(s => s.GameMaster)
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session == null)
            return null;

        if (!session.Players.Any(p => p.PlayerId == playerId))
        {
            _db.SessionPlayers.Add(new SessionPlayer
            {
                SessionId = sessionId,
                PlayerId = playerId
            });

            await _db.SaveChangesAsync();

            await _db.Entry(session)
                .Collection(s => s.Players)
                .Query().Include(sp => sp.Player)
                .LoadAsync();
        }

        return new GetSessionDto
        {
            Id = session.Id,
            Name = session.Name,
            Description = session.Description,
            ScheduledDate = session.ScheduledDate,
            Status = session.Status,
            GameMasterId = session.GameMaster.Id,
            GameMasterName = session.GameMaster.UserName,
            Players = session.Players.Select(p => new SessionPlayerDto
            {
                PlayerId = p.Player.Id,
                PlayerName = p.Player.UserName
            }).ToList()
        };
    }
}
