using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs.Resource;
using TtrpgHelperBackend.Models.Resource;

namespace TtrpgHelperBackend.Services.Resource;

public interface IItemService
{
    Task<GetScenarioItemDto?> GetItem(int itemId, int gameMasterId);
    Task<IEnumerable<GetScenarioItemDto>> GetItems(int campaignId, int gameMasterId);
    Task<GetScenarioItemDto?> CreateItem(CreateItemDto dto, int gameMasterId);
    Task<GetScenarioItemDto?> UpdateItem(UpdateItemDto dto, int gameMasterId);
    Task<bool> DeleteItem(int itemId, int gameMasterId);
}

public class ItemService :  IItemService
{
    private readonly ApplicationDbContext _db;

    public ItemService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<GetScenarioItemDto?> GetItem(int itemId, int gameMasterId)
    {
        var item = await _db.Items
            .Include(i => i.Campaign)
            .FirstOrDefaultAsync(i => i.Id == itemId);
        if (item == null || item.Campaign.GameMasterId != gameMasterId) return null;
        
        return Dto(item);
    }
    
    public async Task<IEnumerable<GetScenarioItemDto>> GetItems(int campaignId, int gameMasterId)
    {
        var items = await _db.Items
            .Include(i => i.Campaign)
            .Where(i => i.CampaignId == campaignId && i.Campaign.GameMasterId == gameMasterId)
            .ToListAsync();
        
        return items.Select(Dto).ToList();
    }
    
    public async Task<GetScenarioItemDto?> CreateItem(CreateItemDto dto, int gameMasterId)
    {
        var campaign = await _db.Campaigns
            .FirstOrDefaultAsync(c => c.Id == dto.CampaignId);
        if (campaign == null || campaign.GameMasterId != gameMasterId) return null;

        var item = new Item
        {
            CampaignId = dto.CampaignId,
            Name = dto.Name,
            Description = dto.Description,
            Type = dto.Type,
            Value = dto.Value,
        };
        
        _db.Items.Add(item);
        await _db.SaveChangesAsync();
        
        return Dto(item);
    }
    
    public async Task<GetScenarioItemDto?> UpdateItem(UpdateItemDto dto, int gameMasterId)
    {
        var item = await _db.Items
            .Include(n => n.Campaign)
            .FirstOrDefaultAsync(n => n.Id == dto.Id);
        if (item == null || item.Campaign.GameMasterId != gameMasterId) return null;
        
        item.Name = dto.Name;
        item.Description = dto.Description;
        item.Type = dto.Type;
        item.Value = dto.Value;
        
        await _db.SaveChangesAsync();
        
        return Dto(item);
    }
    
    public async Task<bool> DeleteItem(int itemId, int gameMasterId)
    {
        var item = await _db.Items
            .Include(i => i.Campaign)
            .FirstOrDefaultAsync(i => i.Id == itemId);
        if (item == null || item.Campaign.GameMasterId != gameMasterId) return false;
        
        _db.Items.Remove(item);
        await _db.SaveChangesAsync();
        
        return true;
    }
    
    
    // ====================
    // -- HELPER METHODS --
    private static GetScenarioItemDto Dto(Item item)
    {
        return new GetScenarioItemDto
        {
            Id = item.Id,
            CampaignId = item.CampaignId,
            Name = item.Name,
            Description = item.Description,
            Type = item.Type ?? "",
            Value = item.Value ?? 0,
        };
    }
}
