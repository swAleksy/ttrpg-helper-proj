using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs.Resource.Item;
using TtrpgHelperBackend.DTOs.Resource.Location;
using TtrpgHelperBackend.DTOs.Resource.Note;
using TtrpgHelperBackend.DTOs.Resource.Npc;
using TtrpgHelperBackend.DTOs.Resource.Scenario;
using TtrpgHelperBackend.Models.Resource;

namespace TtrpgHelperBackend.Services.Resource;

public interface IScenarioService
{
    Task<GetScenarioDto?> GetScenario(int id, int gameMasterId);
    Task<IEnumerable<GetScenarioDto>> GetScenarios(int campaignId, int gameMasterId);
    Task<GetScenarioDto?> CreateScenario(CreateScenarioDto dto, int gameMasterId);
    Task<GetScenarioDto?> UpdateScenario(UpdateScenarioDto dto, int gameMasterId);
    Task<bool> DeleteScenario(int id, int gameMasterId);
}

public class ScenarioService  : IScenarioService
{
    private readonly ApplicationDbContext _db;

    public ScenarioService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<GetScenarioDto?> GetScenario(int id, int gameMasterId)
    {
        var scenario = await _db.Scenarios
            .Include(s => s.Campaign)
                .ThenInclude(c => c.GameMaster)
            // NOTATKI
            .Include(s => s.Chapters)
                .ThenInclude(ch => ch.Notes)
            // PRZEDMIOTY
            .Include(s => s.Chapters)
                .ThenInclude(ch => ch.Items)
            // LOKACJE
            .Include(s => s.Chapters)
                .ThenInclude(ch => ch.Locations)
            // NPC'ty
            .Include(s => s.Chapters)
                .ThenInclude(ch => ch.Npcs)
                    .ThenInclude(n => n.Npc)
                        .ThenInclude(npc => npc.Skills)
            .FirstOrDefaultAsync(s => s.Id == id);
        
        if (scenario == null) return null;
        
        if (scenario.Campaign.GameMasterId != gameMasterId) return null;

        return Dto(scenario);
    }
    
    public async Task<IEnumerable<GetScenarioDto>> GetScenarios(int campaignId, int gameMasterId)
    {
        var scenarios = await _db.Scenarios
            .Where(s => s.CampaignId == campaignId && s.Campaign.GameMasterId == gameMasterId)
            .Include(s => s.Chapters)
            .ToListAsync();
        
        return scenarios.Select(Dto).ToList();
    }

    public async Task<GetScenarioDto?> CreateScenario(CreateScenarioDto dto, int gameMasterId)
    {
        var campaign = await _db.Campaigns
            .FirstOrDefaultAsync(c => c.Id == dto.CampaignId);
        
        if (campaign == null || campaign.GameMasterId != gameMasterId) return null;

        var scenario = new Scenario
        {
            CampaignId = dto.CampaignId,
            Name = dto.Name,
            Description = dto.Description ?? "",
        };

        _db.Scenarios.Add(scenario);
        await _db.SaveChangesAsync();

        scenario.Chapters = new List<ScenarioChapter>();
        
        return Dto(scenario);
    }

    public async Task<GetScenarioDto?> UpdateScenario(UpdateScenarioDto dto, int gameMasterId)
    {
        var scenario = await _db.Scenarios
            .Include(s => s.Campaign)
            .Include(s => s.Chapters)
            .FirstOrDefaultAsync(s => s.Id == dto.Id);
        
        if (scenario == null || scenario.Campaign.GameMasterId != gameMasterId) return null;
        
        scenario.Name = dto.Name;
        scenario.Description = dto.Description ?? "";
        
        await _db.SaveChangesAsync();
        
        return Dto(scenario);
    }

    public async Task<bool> DeleteScenario(int id, int gameMasterId)
    {
        var scenario = await _db.Scenarios
            .Include(s => s.Campaign)
            .FirstOrDefaultAsync(s => s.Id == id);
        
        if (scenario == null || scenario.Campaign.GameMasterId != gameMasterId) return false;
        
        _db.Scenarios.Remove(scenario);
        await _db.SaveChangesAsync();
        
        return true;
    }
    
    
    private static GetScenarioDto Dto(Scenario scenario)
    {
        return new GetScenarioDto
        {
            Id = scenario.Id,
            CampaignId = scenario.CampaignId,
            Name = scenario.Name,
            Description = scenario.Description,
            
            Chapters = scenario.Chapters.Select(ch => new GetScenarioChapterDto
            {
                Id = ch.Id,
                Name = ch.Name,
                ContentMarkdown = ch.ContentMarkdown,
                
                Notes = ch.Notes.Select(n => new GetScenarioNoteDto
                {
                    Id = n.Note.Id,
                    Name = n.Note.Name,
                    ContentMarkdown = n.Note.ContentMarkdown
                }).ToList(),
                
                Items = ch.Items.Select(i => new GetScenarioItemDto
                {
                    Id = i.Item.Id,
                    CampaignId = i.Item.CampaignId,
                    Name = i.Item.Name,
                    Description = i.Item.Description,
                    Type = i.Item.Type ?? "",
                    Value = i.Item.Value ?? 0
                }).ToList(),
                
                Locations = ch.Locations.Select(l => new GetScenarioLocationDto
                {
                    Id = l.Location.Id,
                    CampaignId = l.Location.CampaignId,
                    Name = l.Location.Name,
                    Region = l.Location.Region ?? "",
                    Description = l.Location.Description
                }).ToList(),
                
                Npcs = ch.Npcs.Select(n => new GetScenarioNpcDto
                {
                    Id = n.Npc.Id,
                    CampaignId = n.Npc.CampaignId,
                    Name = n.Npc.Name,
                    Description = n.Npc.Description,
                    Race = n.Npc.Race?.Name ?? "",
                    Class = n.Npc.Class?.Name ?? "",
                    Level = n.Npc.Level,
                    Strength = n.Npc.Strength,
                    Dexterity = n.Npc.Dexterity,
                    Constitution = n.Npc.Constitution,
                    Intelligence = n.Npc.Intelligence,
                    Wisdom = n.Npc.Wisdom,
                    Charisma = n.Npc.Charisma,
                    
                    Skills = n.Npc.Skills.Select(s => new GetScenarioNpcSkillDto
                    {
                        Id = s.Id,
                        NpcId = s.NpcId,
                        Name = s.Name,
                        Description = s.Description,
                        Value = s.Value
                    }).ToList()

                }).ToList()
                
            }).ToList()
        };
    }
}
