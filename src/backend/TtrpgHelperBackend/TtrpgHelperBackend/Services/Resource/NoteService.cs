using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs.Resource;
using TtrpgHelperBackend.Models.Resource;

namespace TtrpgHelperBackend.Services.Resource;

public interface INoteService
{
    Task<GetScenarioNoteDto?> GetNote(int noteId, int gameMasterId);
    Task<IEnumerable<GetScenarioNoteDto>> GetNotes(int campaignId, int gameMasterId);
    Task<GetScenarioNoteDto?> CreateNote(CreateNoteDto dto, int gameMasterId);
    Task<GetScenarioNoteDto?> UpdateNote(UpdateNoteDto dto, int gameMasterId);
    Task<bool> DeleteNote(int noteId, int gameMasterId);
}

public class NoteService :  INoteService
{
    private readonly ApplicationDbContext _db;

    public NoteService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<GetScenarioNoteDto?> GetNote(int noteId, int gameMasterId)
    {
        var note = await _db.Notes
            .Include(n => n.Campaign)
            .FirstOrDefaultAsync(n => n.Id == noteId);
        if (note == null || note.Campaign.GameMasterId != gameMasterId) return null;

        return Dto(note);
    }

    public async Task<IEnumerable<GetScenarioNoteDto>> GetNotes(int campaignId, int gameMasterId)
    {
        var notes = await _db.Notes
            .Include(n => n.Campaign)
            .Where(n => n.CampaignId == campaignId && n.Campaign.GameMasterId == gameMasterId)
            .ToListAsync();
        
        return notes.Select(Dto).ToList();
    }

    public async Task<GetScenarioNoteDto?> CreateNote(CreateNoteDto dto, int gameMasterId)
    {
        var campaign = await _db.Campaigns
            .FirstOrDefaultAsync(c => c.Id == dto.CampaignId);
        if (campaign == null || campaign.GameMasterId != gameMasterId) return null;

        var note = new Note
        {
            CampaignId = dto.CampaignId,
            Name = dto.Name,
            ContentMarkdown = dto.ContentMarkdown,
        };
        
        _db.Notes.Add(note);
        await _db.SaveChangesAsync();
        
        return Dto(note);
    }

    public async Task<GetScenarioNoteDto?> UpdateNote(UpdateNoteDto dto, int gameMasterId)
    {
        var note = await _db.Notes
            .Include(n => n.Campaign)
            .FirstOrDefaultAsync(n => n.Id == dto.Id);
        if (note == null || note.Campaign.GameMasterId != gameMasterId) return null;
        
        note.Name = dto.Name;
        note.ContentMarkdown = dto.ContentMarkdown;
        
        await _db.SaveChangesAsync();
        
        return Dto(note);
    }

    public async Task<bool> DeleteNote(int noteId, int gameMasterId)
    {
        var note = await _db.Notes
            .Include(n => n.Campaign)
            .FirstOrDefaultAsync(n => n.Id == noteId);
        if (note == null || note.Campaign.GameMasterId != gameMasterId) return false;
        
        _db.Notes.Remove(note);
        await _db.SaveChangesAsync();
        
        return true;
    }
    
    
    // ====================
    // -- HELPER METHODS --
    private static GetScenarioNoteDto Dto(Note note)
    {
        return new GetScenarioNoteDto
        {
            Id = note.Id,
            CampaignId = note.CampaignId,
            Name = note.Name,
            ContentMarkdown = note.ContentMarkdown,
        };
    }
}
