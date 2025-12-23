using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs.Session;
using TtrpgHelperBackend.Models.Session;

namespace TtrpgHelperBackend.Services.Session;

public interface ICampaignService
{
    Task<GetCampaignDto?> GetCampaignForGm(int id, int gameMasterId);
    Task<IEnumerable<GetCampaignDto>> GetCampaignsForGm(int gameMasterId, string? status = null);
    
    Task<GetCampaignDto?> GetCampaignForPlayer(int id, int playerId);
    Task<IEnumerable<GetCampaignDto>> GetCampaignsForPlayer(int playerId, string? status = null);
    
    Task<GetCampaignDto> CreateCampaign(CreateCampaignDto dto);
    Task<bool> ArchiveCampaign(int id);
    Task<bool> DeleteCampaign(int id);
}

public class CampaignService : ICampaignService
{
    private readonly ApplicationDbContext _db;

    public CampaignService(ApplicationDbContext db)
    {
        _db = db;
    }
    
    // -- GAMEMASTER --
    public async Task<GetCampaignDto?> GetCampaignForGm(int id,  int gameMasterId)
    {
        var campaign = await _db.Campaigns
            .Where(c => c.GameMasterId == gameMasterId)
            .Include(c => c.GameMaster)
            .Include(c => c.Sessions)
                .ThenInclude(s => s.Players)
                    .ThenInclude(sp => sp.Player)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (campaign == null) return null;

        return Dto(campaign);
    }
    
    public async Task<IEnumerable<GetCampaignDto>> GetCampaignsForGm(int gameMasterId, string? status = null)
    {
        var query = _db.Campaigns
            .Where(c => c.GameMasterId == gameMasterId);

        if (!string.IsNullOrEmpty(status)) query = query.Where(c => c.Status == status);

        query = query
            .Include(c => c.GameMaster)
            .Include(c => c.Sessions)
                .ThenInclude(s => s.Players)
                    .ThenInclude(p => p.Player);

        return await query
            .Select(c => Dto(c))
            .ToListAsync();
    }
    // -- GAMEMASTER --
    
    // -- PLAYER --
    public async Task<GetCampaignDto?> GetCampaignForPlayer(int id,  int playerId)
    {
        var campaign = await _db.Campaigns
            .Where(c => 
                c.Id == id &&
                c.Sessions.Any(s => s.Players.Any(p => p.PlayerId == playerId))
            )
            .Include(c => c.GameMaster)
            .Include(c => c.Sessions)
                .ThenInclude(s => s.Players)
                    .ThenInclude(sp => sp.Player)
            .FirstOrDefaultAsync();

        if (campaign == null) return null;

        return Dto(campaign);
    }
    
    public async Task<IEnumerable<GetCampaignDto>> GetCampaignsForPlayer(int playerId, string? status = null)
    {
        // var query = _db.Campaigns
        //     .Where(c =>
        //         !c.Sessions.Any() ||
        //         c.Sessions.Any(s => s.Players.Any(p => p.PlayerId == playerId)));

        var query = _db.Campaigns
            .Where(c => c.Sessions.Any(s => s.Players.Any(p => p.PlayerId == playerId)));
        
        if (!string.IsNullOrEmpty(status)) query = query.Where(c => c.Status == status);

        query = query
            .Include(c => c.GameMaster)
            .Include(c => c.Sessions)
                .ThenInclude(s => s.Players)
                    .ThenInclude(p => p.Player);

        return await query
            .Select(c => Dto(c))
            .ToListAsync();
    }
    // -- PLAYER --
    
    public async Task<GetCampaignDto> CreateCampaign(CreateCampaignDto dto)
    {
        var campaign = new Campaign
        {
            Name = dto.Name,
            Description = dto.Description,
            GameMasterId = dto.GameMasterId,
        };

        _db.Campaigns.Add(campaign);
        await _db.SaveChangesAsync();

        return new GetCampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Description = campaign.Description,
            GameMasterId = campaign.GameMasterId,
            GameMasterName = (await _db.Users.FindAsync(campaign.GameMasterId))?.UserName ?? "",
            Sessions = new List<GetSessionDto>()
        };
    }

    public async Task<bool> ArchiveCampaign(int id)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();

        var campaign = await _db.Campaigns
            .Include(c => c.Sessions)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (campaign == null) return false;
        
        campaign.Status = "Archived";

        foreach (var session in campaign.Sessions)
        {
            if (!string.Equals(session.Status, "Archived", StringComparison.Ordinal))
            {
                session.Status = "Archived";
            }
        }
        
        await _db.SaveChangesAsync();
        await transaction.CommitAsync();
        
        return true;
    }

    public async Task<bool> DeleteCampaign(int id)
    {
        var campaign = await _db.Campaigns.FindAsync(id);
        if (campaign == null) return false;

        _db.Campaigns.Remove(campaign);
        await _db.SaveChangesAsync();
        
        return true;
    } 
    
    private static GetCampaignDto Dto(Campaign c)
    {
        return new GetCampaignDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            GameMasterId = c.GameMasterId,
            GameMasterName = c.GameMaster.UserName,
            Sessions = c.Sessions.Select(s => new GetSessionDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                ScheduledDate = s.ScheduledDate,
                Status = s.Status,
                Players = s.Players.Select(p => new SessionPlayerDto
                {
                    PlayerId = p.PlayerId,
                    PlayerName = p.Player.UserName
                }).ToList()
            }).ToList()
        };
    }
}
