using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs.Resource.Background;
using TtrpgHelperBackend.Models.Resource;

namespace TtrpgHelperBackend.Services.Resource;

public interface IBackgroundService
{
    Task<GetBackgroundDto?> GetBackground(int backgroundId);
    Task<IEnumerable<GetBackgroundDto>> GetBackgrounds();
    Task<GetBackgroundDto?> CreateBackground(CreateBackgroundDto dto);
    Task<GetBackgroundDto?> UpdateBackground(UpdateBackgroundDto dto);
    Task<bool> DeleteBackground(int backgroundId);
}

public class BackgroundService : IBackgroundService
{
    private readonly ApplicationDbContext _db;

    public BackgroundService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<GetBackgroundDto?> GetBackground(int backgroundId)
    {
        var background = await _db.Backgrounds
            .FirstOrDefaultAsync(b => b.Id == backgroundId);
        if (background == null) return null;
        
        return Dto(background);
    }

    public async Task<IEnumerable<GetBackgroundDto>> GetBackgrounds()
    {
        var backgrounds = await _db.Backgrounds
            .ToListAsync();
        
        return backgrounds.Select(Dto).ToList();
    }

    public async Task<GetBackgroundDto?> CreateBackground(CreateBackgroundDto dto)
    {
        var background = new Background
        {
            Name = dto.Name,
            Description = dto.Description,
        };
        
        _db.Backgrounds.Add(background);
        await _db.SaveChangesAsync();
        
        return Dto(background);
    }

    public async Task<GetBackgroundDto?> UpdateBackground(UpdateBackgroundDto dto)
    {
        var background = await _db.Backgrounds
            .FirstOrDefaultAsync(b => b.Id == dto.Id);
        if (background == null) return null;

        background.Name = dto.Name;
        background.Description = dto.Description;
        
        await _db.SaveChangesAsync();
        
        return Dto(background);
    }

    public async Task<bool> DeleteBackground(int backgroundId)
    {
        var deleted = await _db.Backgrounds
            .FirstOrDefaultAsync(b => b.Id == backgroundId);
        if (deleted == null) return false;
        
        _db.Backgrounds.Remove(deleted);
        await _db.SaveChangesAsync();
        
        return true;
    }
    
    
    // ====================
    // -- HELPER METHODS --
    private static GetBackgroundDto Dto(Background background)
    {
        return new GetBackgroundDto
        {
            Id = background.Id,
            Name = background.Name,
            Description = background.Description,
        };
    }
}
