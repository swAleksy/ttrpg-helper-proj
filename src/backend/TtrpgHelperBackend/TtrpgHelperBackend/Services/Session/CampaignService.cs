using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs.Session;
using TtrpgHelperBackend.Models.Session;

namespace TtrpgHelperBackend.Services.Session;

public interface ICampaignService
{
    Task<GetCampaignDto?> GetCampaign(int id);
    Task<IEnumerable<GetCampaignDto>> GetCampaigns();
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

    public async Task<GetCampaignDto?> GetCampaign(int id)
    {
        var campaign = await _db.Campaigns
            .Include(c => c.Sessions)
                .ThenInclude(s => s.GameMaster)
            .Include(c => c.Sessions)
                .ThenInclude(s => s.Players)
                    .ThenInclude(sp => sp.Player)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (campaign == null) return null;

        return new GetCampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Description = campaign.Description,
            Sessions = campaign.Sessions.Select(s => new GetSessionDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                ScheduledDate = s.ScheduledDate,
                Status = s.Status,
                GameMasterName = s.GameMaster.UserName,
                Players = s.Players.Select(p => new SessionPlayerDto
                {
                    PlayerId = p.PlayerId,
                    PlayerName = p.Player.UserName
                }).ToList()
            }).ToList()
        };
    }
    
    public async Task<IEnumerable<GetCampaignDto>> GetCampaigns()
    {
        return await _db.Campaigns
            .Include(c => c.Sessions)
                .ThenInclude(s => s.GameMaster)
            .Include(c => c.Sessions)
                .ThenInclude(s => s.Players)
                    .ThenInclude(sp => sp.Player)
            .Select(c => new GetCampaignDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Sessions = c.Sessions.Select(s => new GetSessionDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    ScheduledDate = s.ScheduledDate,
                    Status = s.Status,
                    GameMasterName = s.GameMaster.UserName,
                    Players = s.Players.Select(p => new SessionPlayerDto
                    {
                        PlayerId = p.PlayerId,
                        PlayerName = p.Player.UserName
                    }).ToList()
                }).ToList()
            })
            .ToListAsync();
    }

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
            Sessions = new List<GetSessionDto>()
        };
    }

    public async Task<bool> ArchiveCampaign(int id)
    {
        var campaign = await _db.Campaigns.FindAsync(id);
        if (campaign == null) return false;
        
        campaign.Status = "Archived";
        
        _db.Update(campaign);
        await _db.SaveChangesAsync();
        
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
}
