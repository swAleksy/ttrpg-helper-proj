using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs.Resource;
using TtrpgHelperBackend.Models.Resource;

namespace TtrpgHelperBackend.Services.Resource;

public interface IScenarioChapterService
{
    Task<GetScenarioChapterDto?> GetScenarioChapter(int id, int gameMasterId);
    Task<IEnumerable<GetScenarioChapterDto>> GetScenarioChapters(int scenarioId, int gameMasterId);
}

public class ScenarioChapterService : IScenarioChapterService
{
    private readonly ApplicationDbContext _db;

    public ScenarioChapterService(ApplicationDbContext db)
    {
        _db = db;
    }
    
    public async Task<GetScenarioChapterDto?> GetScenarioChapter(int id, int gameMasterId)
    {
        var chapter = await _db.ScenarioChapters
            .Include(ch => ch.Scenario)
                .ThenInclude(s => s.Campaign)
                    .ThenInclude(c => c.GameMaster)
            // NOTATKI
            .Include(ch => ch.Notes)
                .ThenInclude(n => n.Note)
            // PRZEDMIOTY
            .Include(ch => ch.Items)
                .ThenInclude(i => i.Item)
            // LOKACJE
            .Include(ch => ch.Locations)
                .ThenInclude(l => l.Location)
            // NPC'ty
            .Include(ch => ch.Npcs)
                .ThenInclude(n => n.Npc)
                    .ThenInclude(npc => npc.Skills)
            .FirstOrDefaultAsync(ch => ch.Id == id);
            
        if (chapter == null || chapter.Scenario.Campaign.GameMasterId != gameMasterId) return null;

        return Dto(chapter);
    }

    public async Task<IEnumerable<GetScenarioChapterDto>> GetScenarioChapters(int scenarioId, int gameMasterId)
    {
        var scenario = await _db.Scenarios
            .Include(s => s.Campaign)
            .FirstOrDefaultAsync(s => s.Id == scenarioId);
        
        if (scenario == null || scenario.Campaign.GameMasterId != gameMasterId) return Enumerable.Empty<GetScenarioChapterDto>();
        
        var chapters = await _db.ScenarioChapters
            .Where(ch => ch.ScenarioId == scenarioId)
            // NOTATKI
            .Include(ch => ch.Notes)
                .ThenInclude(n => n.Note)
            // PRZEDMIOTY
            .Include(ch => ch.Items)
                .ThenInclude(i => i.Item)
            //LOKACJE
            .Include(ch => ch.Locations)
                .ThenInclude(l => l.Location)
            // NPC'ty
            .Include(ch => ch.Npcs)
                .ThenInclude(n => n.Npc)
                    .ThenInclude(npc => npc.Skills)
            .ToListAsync();
        
        return chapters.Select(Dto).ToList();
    }
    

    private static GetScenarioChapterDto Dto(ScenarioChapter scenarioChapter)
    {
        return new GetScenarioChapterDto
        {
            Id = scenarioChapter.Id,
            Name = scenarioChapter.Name,
            ContentMarkdown = scenarioChapter.ContentMarkdown,

            Notes = scenarioChapter.Notes.Select(n => new GetScenarioNoteDto
            {
                Id = n.Note.Id,
                CampaignId = n.Note.CampaignId,
                Name = n.Note.Name,
                ContentMarkdown = n.Note.ContentMarkdown
            }).ToList(),

            Items = scenarioChapter.Items.Select(i => new GetScenarioItemDto
            {
                Id = i.Item.Id,
                CampaignId = i.Item.CampaignId,
                Name = i.Item.Name,
                Description = i.Item.Description,
                Type = i.Item.Type ?? "",
                Value = i.Item.Value ?? 0
            }).ToList(),

            Locations = scenarioChapter.Locations.Select(l => new GetScenarioLocationDto
            {
                Id = l.Location.Id,
                CampaignId = l.Location.CampaignId,
                Name = l.Location.Name,
                Region = l.Location.Region ?? "",
                Description = l.Location.Description
            }).ToList(),

            Npcs = scenarioChapter.Npcs.Select(n => new GetScenarioNpcDto
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
        };
    }
}
