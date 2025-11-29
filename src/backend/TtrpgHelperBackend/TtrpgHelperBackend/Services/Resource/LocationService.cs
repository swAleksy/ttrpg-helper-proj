using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs.Resource;
using TtrpgHelperBackend.Models.Resource;

namespace TtrpgHelperBackend.Services.Resource;

public interface ILocationService
{
    Task<GetScenarioLocationDto?> GetLocation(int locationId, int gameMasterId);
    Task<IEnumerable<GetScenarioLocationDto>> GetLocations(int campaignId, int gameMasterId);
    Task<GetScenarioLocationDto?> CreateLocation(CreateLocationDto dto, int gameMasterId);
    Task<GetScenarioLocationDto?> UpdateLocation(UpdateLocationDto dto, int gameMasterId);
    Task<bool> DeleteLocation(int locationId, int gameMasterId);
}

public class LocationService :  ILocationService
{
    private readonly ApplicationDbContext _db;

    public LocationService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<GetScenarioLocationDto?> GetLocation(int locationId, int gameMasterId)
    {
        var location = await _db.Locations
            .Include(l => l.Campaign)
            .FirstOrDefaultAsync(l => l.Id == locationId);
        if (location == null || location.Campaign.GameMasterId != gameMasterId) return null;

        return Dto(location);
    }

    public async Task<IEnumerable<GetScenarioLocationDto>> GetLocations(int campaignId, int gameMasterId)
    {
        var locations = await _db.Locations
            .Include(l => l.Campaign)
            .Where(l => l.CampaignId == campaignId && l.Campaign.GameMasterId == gameMasterId)
            .ToListAsync();
        
        return locations.Select(Dto).ToList();
    }

    public async Task<GetScenarioLocationDto?> CreateLocation(CreateLocationDto dto, int gameMasterId)
    {
        var campaign = await _db.Campaigns
            .FirstOrDefaultAsync(c => c.Id == dto.CampaignId);
        if (campaign == null || campaign.GameMasterId != gameMasterId) return null;

        var location = new Location
        {
            CampaignId = dto.CampaignId,
            Name = dto.Name,
            Region = dto.Region,
            Description = dto.Description,
        };
        
        _db.Locations.Add(location);
        await _db.SaveChangesAsync();
        
        return Dto(location);
    }
    
    public async Task<GetScenarioLocationDto?> UpdateLocation(UpdateLocationDto dto, int gameMasterId)
    {
        var location = await _db.Locations
            .Include(l => l.Campaign)
            .FirstOrDefaultAsync(l => l.Id == dto.Id);
        if (location == null || location.Campaign.GameMasterId != gameMasterId) return null;
        
        location.Name =  dto.Name;
        location.Region = dto.Region;
        location.Description = dto.Description;
        
        await _db.SaveChangesAsync();
        
        return Dto(location);
    }

    public async Task<bool> DeleteLocation(int locationId, int gameMasterId)
    {
        var location = await _db.Locations
            .Include(l => l.Campaign)
            .FirstOrDefaultAsync(l => l.Id == locationId);
        if (location == null || location.Campaign.GameMasterId != gameMasterId) return false;
        
        _db.Locations.Remove(location);
        await _db.SaveChangesAsync();
        
        return true;
    }
    
    
    // ====================
    // -- HELPER METHODS --
    private static GetScenarioLocationDto Dto(Location location)
    {
        return new GetScenarioLocationDto
        {
            Id = location.Id,
            CampaignId = location.CampaignId,
            Name = location.Name,
            Region = location.Region ?? "",
            Description = location.Description,
        };
    }
}