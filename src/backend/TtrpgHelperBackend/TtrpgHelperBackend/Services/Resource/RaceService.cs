using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs.Resource.Race;
using TtrpgHelperBackend.Models.Resource;

namespace TtrpgHelperBackend.Services.Resource;

public interface IRaceService
{
    Task<GetRaceDto?> GetRace(int raceId);
    Task<IEnumerable<GetRaceDto>> GetRaces();
    Task<GetRaceDto?> CreateRace(CreateRaceDto dto);
    Task<GetRaceDto?> UpdateRace(UpdateRaceDto dto);
    Task<bool> DeleteRace(int raceId);
}

public class RaceService : IRaceService
{
    private readonly ApplicationDbContext _db;

    public RaceService(ApplicationDbContext db)
    {
        _db = db;
    }
    
    public async Task<GetRaceDto?> GetRace(int raceId)
    {
        var race = await _db.Races
            .FirstOrDefaultAsync(r => r.Id == raceId);
        if (race == null) return null;

        return Dto(race);
    }

    public async Task<IEnumerable<GetRaceDto>> GetRaces()
    {
        var races = await _db.Races
            .ToListAsync();
        
        return races.Select(Dto).ToList();
    }

    public async Task<GetRaceDto?> CreateRace(CreateRaceDto dto)
    {
        var race = new Race
        {
            Name = dto.Name,
            Description = dto.Description,
        };
        
        _db.Races.Add(race);
        await _db.SaveChangesAsync();
        
        return Dto(race);
    }

    public async Task<GetRaceDto?> UpdateRace(UpdateRaceDto dto)
    {
        var race = await _db.Races
            .FirstOrDefaultAsync(r => r.Id == dto.Id);
        if (race == null) return null;
        
        race.Name = dto.Name;
        race.Description = dto.Description;
        
        await _db.SaveChangesAsync();
        
        return Dto(race);
    }

    public async Task<bool> DeleteRace(int raceId)
    {
        var deleted = await _db.Races
            .FirstOrDefaultAsync(r => r.Id == raceId);
        if (deleted == null) return false;
        
        _db.Races.Remove(deleted);
        await _db.SaveChangesAsync();
        
        return true;
    }
    
    
    // ====================
    // -- HELPER METHODS --
    private static GetRaceDto Dto(Race race)
    {
        return new GetRaceDto
        {
            Id = race.Id,
            Name = race.Name,
            Description = race.Description,
        };
    }
}
