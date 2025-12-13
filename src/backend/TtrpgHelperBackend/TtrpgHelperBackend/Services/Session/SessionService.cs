using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs.Session;
using TtrpgHelperBackend.Models.Session;
using SessionModel = TtrpgHelperBackend.Models.Session.Session;

namespace TtrpgHelperBackend.Services.Session;

public interface ISessionService
{
    // ===========
    // -- ADMIN --
    Task<GetSessionDto?> GetSessionForAdmin(int id);
    Task<IEnumerable<GetSessionDto>> GetSessionsForAdmin(string? status = null);
    Task<bool> DeleteSessionForAdmin(int id);
    
    // ============
    // -- PLAYER --
    Task<GetSessionDto?> GetSessionForPlayer(int id, int playerId);
    Task<IEnumerable<GetSessionDto>> GetSessionsForPlayer(int playerId, string? status = null);
    
    // ================
    // -- GAMEMASTER --
    Task<GetSessionDto?> GetSessionForGm(int id, int gameMasterId);
    Task<IEnumerable<GetSessionDto>> GetSessionsForGm(int gameMasterId, string? status = null);
    Task<GetSessionDto?> CreateSession(CreateSessionDto dto, int gameMasterId);
    Task<GetSessionDto?> UpdateSession(UpdateSessionDto dto, int gameMasterId);
    Task<bool> ArchiveSession(int id, int gameMasterId);
    Task<bool> DeleteSession(int id, int gameMasterId);
    Task<GetSessionDto?> AddPlayer(int sessionId, int gameMasterId, int playerId);
}

public class SessionService : ISessionService
{
    private readonly ApplicationDbContext _db;

    public SessionService(ApplicationDbContext db)
    {
        _db = db;
    }
    
    // ===========
    // -- ADMIN --
    public async Task<GetSessionDto?> GetSessionForAdmin(int id)
    {
        var session = await _db.Sessions
            .Include(s => s.Campaign)
                .ThenInclude(c => c.GameMaster)
            .Include(s => s.Players)
                .ThenInclude(p => p.Player)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (session == null) return null;

        return Dto(session);
    }
    
    public async Task<IEnumerable<GetSessionDto>> GetSessionsForAdmin(string? status = null)
    {
        var query = _db.Sessions.AsQueryable();
        if (!string.IsNullOrEmpty(status)) query = query.Where(s => s.Status == status);
        
        query = query
            .Include(s => s.Campaign)
                .ThenInclude(c => c.GameMaster)
            .Include(s => s.Players)
                .ThenInclude(p => p.Player);
        
        return await query
            .Select(s => Dto(s))
            .ToListAsync();
    }
    
    public async Task<bool> DeleteSessionForAdmin(int id)
    {
        var session = await _db.Sessions.FindAsync(id);
        if (session == null) return false;

        _db.Sessions.Remove(session);
        await _db.SaveChangesAsync();
        
        return true;
    }
    
    
    // ============
    // -- PLAYER --
    public async Task<GetSessionDto?> GetSessionForPlayer(int id, int playerId)
    {
        var session = await _db.Sessions
            .Include(s => s.Players).ThenInclude(p => p.Player)
            .Include(s => s.Campaign).ThenInclude(c => c.GameMaster)
            .FirstOrDefaultAsync(s => s.Id == id && s.Players.Any(p => p.PlayerId == playerId));

        return session == null ? null : Dto(session);
    }

    public async Task<IEnumerable<GetSessionDto>> GetSessionsForPlayer(int playerId, string? status = null)
    {
        var query = _db.Sessions
            .Where(s => s.Players.Any(p => p.PlayerId == playerId));
        
        if (!string.IsNullOrEmpty(status)) query = query.Where(s => s.Status == status);
        
        query = query
            .Include(s => s.Players)
                .ThenInclude(sp => sp.Player);
        
        return await query
            .Select(s => Dto(s))
            .ToListAsync();
    }
    
    
    // ================
    // -- GAMEMASTER --
    public async Task<GetSessionDto?> GetSessionForGm(int id, int gameMasterId)
    {
        var session = await _db.Sessions
            .Include(s => s.Campaign)
            .ThenInclude(c => c.GameMaster)
            .Include(s => s.Players)
            .ThenInclude(sp => sp.Player)
            .FirstOrDefaultAsync(s => s.Id == id && s.Campaign.GameMasterId == gameMasterId);

        return session == null ? null : Dto(session);
    }

    public async Task<IEnumerable<GetSessionDto>> GetSessionsForGm(int gameMasterId, string? status = null)
    {
        var query = _db.Sessions
            .Where(s => s.Campaign.GameMasterId == gameMasterId);
        
        if (!string.IsNullOrEmpty(status)) query = query.Where(s => s.Status == status);
        
        query = query
            .Include(s => s.Players)
            .ThenInclude(p => p.Player);
        
        return await query
            .Select(s => Dto(s))
            .ToListAsync();
    }
    public async Task<GetSessionDto?> CreateSession(CreateSessionDto dto, int gameMasterId)
    {
        var campaign = await _db.Campaigns
            .FirstOrDefaultAsync(c => c.Id == dto.CampaignId);
        if (campaign == null || campaign.GameMasterId != gameMasterId) return null;
        
        var session = new SessionModel
        {
            Name = dto.Name,
            Description = dto.Description,
            ScheduledDate = dto.ScheduledDate,
            Status = dto.Status,
            CampaignId = dto.CampaignId
        };

        _db.Sessions.Add(session);
        await _db.SaveChangesAsync();

        return Dto(session);
    }

    public async Task<GetSessionDto?> UpdateSession(UpdateSessionDto dto, int gameMasterId)
    {
        var session = await _db.Sessions
            .Include(s => s.Campaign)
            .FirstOrDefaultAsync(s => s.Id == dto.Id && s.Campaign.GameMasterId == gameMasterId);

        if (session == null) return null;

        session.Name = dto.Name;
        session.Description = dto.Description;
        session.ScheduledDate = dto.ScheduledDate;
        session.Status = dto.Status;

        await _db.SaveChangesAsync();

        return Dto(session);
    }

    public async Task<bool> ArchiveSession(int id, int gameMasterId)
    {
        var session = await _db.Sessions
            .Include(s => s.Campaign)
            .FirstOrDefaultAsync(s => s.Id == id && s.Campaign.GameMasterId == gameMasterId);
        if (session == null) return false;
        
        session.Status = "Archived";
        await _db.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> DeleteSession(int id, int gameMasterId)
    {
        var session = await _db.Sessions
            .Include(s => s.Campaign)
            .FirstOrDefaultAsync(s => s.Id == id && s.Campaign.GameMasterId == gameMasterId);
        if (session == null) return false;

        _db.Sessions.Remove(session);
        await _db.SaveChangesAsync();
        
        return true;
    }

    public async Task<GetSessionDto?> AddPlayer(int sessionId, int gameMasterId, int playerId)
    {
        var session = await _db.Sessions
            .Include(s => s.Campaign)
            .FirstOrDefaultAsync(s => s.Id == sessionId);
        if (session == null || session.Campaign.GameMasterId != gameMasterId) return null;

        if (!session.Players.Any(p => p.PlayerId == playerId))
        {
            _db.SessionPlayers.Add(new SessionPlayer
            {
                SessionId = sessionId,
                PlayerId = playerId
            });

            await _db.SaveChangesAsync();
        }
        
        session = await _db.Sessions
            .Include(s => s.Players)
                .ThenInclude(sp => sp.Player)
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        return Dto(session!);
    }
    
    
    // ====================
    // -- HELPER METHODS --
    private static GetSessionDto Dto(SessionModel session)
    {
        return new GetSessionDto
        {
            Id = session.Id,
            Name = session.Name,
            Description = session.Description,
            ScheduledDate = session.ScheduledDate,
            Status = session.Status,
            Players = session.Players.Select(p => new SessionPlayerDto
            {
                PlayerId = p.PlayerId,
                PlayerName = p.Player.UserName
            }).ToList()
        };
    }
}
