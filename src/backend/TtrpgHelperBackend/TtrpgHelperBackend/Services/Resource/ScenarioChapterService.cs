using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs.Resource.Item;
using TtrpgHelperBackend.DTOs.Resource.Location;
using TtrpgHelperBackend.DTOs.Resource.Note;
using TtrpgHelperBackend.DTOs.Resource.Npc;
using TtrpgHelperBackend.DTOs.Resource.Scenario;
using TtrpgHelperBackend.Models.Resource;

namespace TtrpgHelperBackend.Services.Resource;

public interface IScenarioChapterService
{
    // ======================
    // -- SCENARIO CHAPTER --
    Task<GetScenarioChapterDto?> GetScenarioChapter(int id, int gameMasterId);
    Task<IEnumerable<GetScenarioChapterDto>> GetScenarioChapters(int scenarioId, int gameMasterId);
    Task<GetScenarioChapterDto?> CreateScenarioChapter(CreateScenarioChapterDto dto, int gameMasterId);
    Task<GetScenarioChapterDto?> UpdateScenarioChapter(UpdateScenarioChapterDto dto, int gameMasterId);
    Task<bool> DeleteScenarioChapter(int id, int gameMasterId);
    
    // ==========
    // -- NOTE --
    Task<GetScenarioNoteDto?> AddNoteToChapter(int chapterId, int noteId, int gameMasterId);
    Task<bool> DeleteNoteFromChapter(int chapterId, int noteId, int gameMasterId);
    
    // ==========
    // -- ITEM --
    Task<GetScenarioItemDto?> AddItemToChapter(int chapterId, int itemId, int gameMasterId);
    Task<bool> DeleteItemFromChapter(int chapterId, int itemId, int gameMasterId);
    
    // ==============
    // -- LOCATION --
    Task<GetScenarioLocationDto?> AddLocationToChapter(int chapterId, int locationId, int gameMasterId);
    Task<bool> DeleteLocationFromChapter(int chapterId, int locationId, int gameMasterId);
    
    // =========
    // -- NPC --
    Task<GetScenarioNpcDto?> AddNpcToChapter(int chapterId, int npcId, int gameMasterId);
    Task<bool> DeleteNpcFromChapter(int chapterId, int npcId, int gameMasterId);
}

public class ScenarioChapterService : IScenarioChapterService
{
    private readonly ApplicationDbContext _db;

    public ScenarioChapterService(ApplicationDbContext db)
    {
        _db = db;
    }
    
    // ======================
    // -- SCENARIO CHAPTER --
    public async Task<GetScenarioChapterDto?> GetScenarioChapter(int id, int gameMasterId)
    {
        var chapter = await IncludeAllChapterData(_db.ScenarioChapters)
            .Include(ch => ch.Scenario)
                .ThenInclude(s => s.Campaign)
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
        
        var chapters = await IncludeAllChapterData(_db.ScenarioChapters)
            .Where(ch => ch.ScenarioId == scenarioId)
            .Include(ch => ch.Scenario)
                .ThenInclude(s => s.Campaign)
            .ToListAsync();
        
        return chapters.Select(Dto).ToList();
    }

    public async Task<GetScenarioChapterDto?> CreateScenarioChapter(CreateScenarioChapterDto dto,  int gameMasterId)
    {
        var scenario = await _db.Scenarios
            .Include(s => s.Campaign)
            .FirstOrDefaultAsync(s => s.Id == dto.ScenarioId);
        if (scenario == null || scenario.Campaign.GameMasterId != gameMasterId) return null;

        var scenarioChapter = new ScenarioChapter
        {
            ScenarioId = dto.ScenarioId,
            Name = dto.Name,
            ContentMarkdown = dto.ContentMarkdown,
        };
        
        _db.ScenarioChapters.Add(scenarioChapter);
        await _db.SaveChangesAsync();
        
        return Dto(scenarioChapter);
    }

    public async Task<GetScenarioChapterDto?> UpdateScenarioChapter(UpdateScenarioChapterDto dto, int gameMasterId)
    {
        var chapter = await IncludeAllChapterData(_db.ScenarioChapters)
            .Include(ch => ch.Scenario)
                .ThenInclude(s => s.Campaign)
            .FirstOrDefaultAsync(ch => ch.Id == dto.Id);
        if (chapter == null || chapter.Scenario.Campaign.GameMasterId != gameMasterId) return null;
        
        chapter.Name = dto.Name;
        chapter.ContentMarkdown = dto.ContentMarkdown;
        
        await _db.SaveChangesAsync();
        
        return Dto(chapter);
    }
    
    public async Task<bool> DeleteScenarioChapter(int id, int gameMasterId)
    {
        var chapter = await IncludeAllChapterData(_db.ScenarioChapters)
            .Include(ch => ch.Scenario)
                .ThenInclude(s => s.Campaign)
            .FirstOrDefaultAsync(ch => ch.Id == id);
        if (chapter == null || chapter.Scenario.Campaign.GameMasterId != gameMasterId) return false;

        _db.ScenarioChapters.Remove(chapter);
        await _db.SaveChangesAsync();
        
        return true;
    }
    // -- SCENARIO CHAPTER --
    // ======================
    
    
    // ==========
    // -- NOTE --
    public async Task<GetScenarioNoteDto?> AddNoteToChapter(int chapterId, int noteId, int gameMasterId)
    {
        var chapter = await GetAuthorizedChapter(chapterId, gameMasterId);
        if (chapter == null) return null;
        
        var note = await _db.Notes
            .FirstOrDefaultAsync(n => n.Id == noteId);
        if (note == null || note.CampaignId != chapter.Scenario.CampaignId) return null;

        var link = new ScenarioChapterNote
        {
            ScenarioChapterId = chapterId,
            NoteId = noteId,
        };
        
        _db.ScenarioChapterNotes.Add(link);
        await _db.SaveChangesAsync();

        return new GetScenarioNoteDto
        {
            Id = note.Id,
            CampaignId = note.CampaignId,
            Name = note.Name,
            ContentMarkdown = note.ContentMarkdown,
        };
    }

    public async Task<bool> DeleteNoteFromChapter(int chapterId, int noteId, int gameMasterId)
    {
        if (await GetAuthorizedChapter(chapterId, gameMasterId) == null) return false;
        
        var link = await _db.ScenarioChapterNotes
            .FirstOrDefaultAsync(x => x.ScenarioChapterId == chapterId && x.NoteId == noteId);
        if (link == null) return false;
        
        _db.ScenarioChapterNotes.Remove(link);
        await _db.SaveChangesAsync();
        
        return true;
    }
    
    
    // ==========
    // -- ITEM --
    public async Task<GetScenarioItemDto?> AddItemToChapter(int chapterId, int itemId, int gameMasterId)
    {
        var chapter = await GetAuthorizedChapter(chapterId, gameMasterId);
        if (chapter == null) return null;
        
        var item = await _db.Items
            .FirstOrDefaultAsync(n => n.Id == itemId);
        if (item == null || item.CampaignId != chapter.Scenario.CampaignId) return null;

        var link = new ScenarioChapterItem
        {
            ScenarioChapterId = chapterId,
            ItemId = itemId,
        };
        
        _db.ScenarioChapterItems.Add(link);
        await _db.SaveChangesAsync();

        return new GetScenarioItemDto
        {
            Id = item.Id,
            CampaignId = item.CampaignId,
            Name = item.Name,
            Description = item.Description,
            Type =  item.Type ?? "",
            Value =  item.Value ?? 0,
        };
    }

    public async Task<bool> DeleteItemFromChapter(int chapterId, int itemId, int gameMasterId)
    {
        if (await GetAuthorizedChapter(chapterId, gameMasterId) == null) return false;
        
        var link = await _db.ScenarioChapterItems
            .FirstOrDefaultAsync(x => x.ScenarioChapterId == chapterId && x.ItemId == itemId);
        if (link == null) return false;
        
        _db.ScenarioChapterItems.Remove(link);
        await _db.SaveChangesAsync();
        
        return true;
    }
    
    
    // ==============
    // -- LOCATION --
    public async Task<GetScenarioLocationDto?> AddLocationToChapter(int chapterId, int locationId, int gameMasterId)
    {
        var chapter = await GetAuthorizedChapter(chapterId, gameMasterId);
        if (chapter == null) return null;
        
        var location = await _db.Locations
            .FirstOrDefaultAsync(n => n.Id == locationId);
        if (location == null || location.CampaignId != chapter.Scenario.CampaignId) return null;

        var link = new ScenarioChapterLocation
        {
            ScenarioChapterId = chapterId,
            LocationId = locationId,
        };
        
        _db.ScenarioChapterLocations.Add(link);
        await _db.SaveChangesAsync();

        return new GetScenarioLocationDto
        {
            Id = location.Id,
            CampaignId = location.CampaignId,
            Name = location.Name,
            Region = location.Region ?? "",
            Description =  location.Description,
        };
    }
    
    public async Task<bool> DeleteLocationFromChapter(int chapterId, int locationId, int gameMasterId)
    {
        if (await GetAuthorizedChapter(chapterId, gameMasterId) == null) return false;
        
        var link = await _db.ScenarioChapterLocations
            .FirstOrDefaultAsync(x => x.ScenarioChapterId == chapterId && x.LocationId == locationId);
        if (link == null) return false;
        
        _db.ScenarioChapterLocations.Remove(link);
        await _db.SaveChangesAsync();
        
        return true;
    }

    
    // =========
    // -- NPC --
    public async Task<GetScenarioNpcDto?> AddNpcToChapter(int chapterId, int npcId, int gameMasterId)
    {
        var chapter = await GetAuthorizedChapter(chapterId, gameMasterId);
        if (chapter == null) return null;
        
        var npc = await _db.Npcs
            .Include(n => n.Skills)
            .Include(n => n.Race)
            .Include(n => n.Class)
            .FirstOrDefaultAsync(n => n.Id == npcId);
        if (npc == null || npc.CampaignId != chapter.Scenario.CampaignId) return null;

        if (await _db.ScenarioChapterNpcs.AnyAsync(l => l.ScenarioChapterId == chapterId && l.NpcId == npcId)) return null;
        
        var link = new ScenarioChapterNpc
        {
            ScenarioChapterId = chapterId,
            NpcId = npcId,
        };
        
        _db.ScenarioChapterNpcs.Add(link);
        await _db.SaveChangesAsync();

        return new GetScenarioNpcDto
        {
            Id = npc.Id,
            CampaignId = npc.CampaignId,
            Name = npc.Name,
            Description = npc.Description,
            Race = npc.Race?.Name ?? "",
            Class = npc.Class?.Name ?? "",
            Level = npc.Level,
            Strength = npc.Strength,
            Dexterity = npc.Dexterity,
            Constitution = npc.Constitution,
            Intelligence = npc.Intelligence,
            Wisdom = npc.Wisdom,
            Charisma = npc.Charisma,
            Skills = npc.Skills.Select(s => new GetScenarioNpcSkillDto
            {
                Id = s.Id,
                NpcId = s.NpcId,
                Name = s.Name,
                Description = s.Description,
                Value = s.Value
            }).ToList()
        };
    }
    
    public async Task<bool> DeleteNpcFromChapter(int chapterId, int npcId, int gameMasterId)
    {
        if (await GetAuthorizedChapter(chapterId, gameMasterId) == null) return false;
        
        var link = await _db.ScenarioChapterNpcs
            .FirstOrDefaultAsync(x => x.ScenarioChapterId == chapterId && x.NpcId == npcId);
        if (link == null) return false;
        
        _db.ScenarioChapterNpcs.Remove(link);
        await _db.SaveChangesAsync();
        
        return true;
    }
    
    
    // ====================
    // -- HELPER METHODS --
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

    private async Task<ScenarioChapter?> GetAuthorizedChapter(int chapterId, int gameMasterId)
    {
        return await _db.ScenarioChapters
            .Include(ch => ch.Scenario)
                .ThenInclude(s => s.Campaign)
            .FirstOrDefaultAsync(ch => ch.Id == chapterId && ch.Scenario.Campaign.GameMasterId == gameMasterId);
        
    }
    
    private IQueryable<ScenarioChapter> IncludeAllChapterData(IQueryable<ScenarioChapter> query)
    {
        return query
            .Include(ch => ch.Notes).ThenInclude(n => n.Note)
            .Include(ch => ch.Items).ThenInclude(i => i.Item)
            .Include(ch => ch.Locations).ThenInclude(l => l.Location)
            .Include(ch => ch.Npcs).ThenInclude(n => n.Npc).ThenInclude(npc => npc.Skills);
    }
}
