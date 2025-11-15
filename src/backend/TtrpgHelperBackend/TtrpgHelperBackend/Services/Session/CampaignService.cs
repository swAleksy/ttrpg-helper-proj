using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs.Session;
using TtrpgHelperBackend.Models.Session;

namespace TtrpgHelperBackend.Services.Session;

public interface ICampaignService
{
    Task<GetCampaignDto?> GetCampaign(int id, int gameMasterId);
    Task<IEnumerable<GetCampaignDto>> GetCampaigns(int gameMasterId);
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

    public async Task<GetCampaignDto?> GetCampaign(int id, int gameMasterId)
    {
        var campaign = await _db.Campaigns
            .Where(c => c.GameMasterId == gameMasterId)
            .Include(c => c.Sessions)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (campaign == null) return null;

        return new GetCampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Description = campaign.Description,
            Sessions = campaign.Sessions.Select(s => new SummarizeSessionDto
            {
                Id = s.Id,
                Name = s.Name,
                Status = s.Status,
                ScheduledDate = s.ScheduledDate
            }).ToList()
        };
    }
    
    public async Task<IEnumerable<GetCampaignDto>> GetCampaigns(int gameMasterId)
    {
        var campaigns = await _db.Campaigns
            .Where(c => c.GameMasterId == gameMasterId)
            .Include(c => c.Sessions)
            .ToListAsync();

        return campaigns.Select(c => new GetCampaignDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            Sessions = c.Sessions.Select(s => new SummarizeSessionDto
            {
                Id = s.Id,
                Name = s.Name,
                Status = s.Status,
                ScheduledDate = s.ScheduledDate
            }).ToList()
        });
    }

    public async Task<GetCampaignDto> CreateCampaign(CreateCampaignDto dto)
    {
        var campaign = new Campaign
        {
            Name = dto.Name,
            Description = dto.Description,
            GameMasterId = dto.GameMasterId,
            Created = DateTime.UtcNow
        };

        _db.Campaigns.Add(campaign);
        await _db.SaveChangesAsync();

        return new GetCampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Description = campaign.Description,
            Sessions = new List<SummarizeSessionDto>()
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