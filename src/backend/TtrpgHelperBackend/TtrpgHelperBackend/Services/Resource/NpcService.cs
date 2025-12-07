using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs.Resource.Npc;
using TtrpgHelperBackend.Models.Resource;


namespace TtrpgHelperBackend.Services.Resource;

public interface INpcService
{
    // ==============
    // -- CAMPAIGN --
    Task<GetScenarioNpcDto?> GetNpc(int npcId, int gameMasterId);
    Task<IEnumerable<GetScenarioNpcDto>> GetNpcs(int campaignId, int gameMasterId);
    Task<GetScenarioNpcDto?> CreateNpc(CreateNpcDto dto, int gameMasterId);
    Task<GetScenarioNpcDto?> UpdateNpc(UpdateNpcDto dto, int gameMasterId);
    Task<bool> DeleteNpc(int npcId, int gameMasterId);
    
    // ==========
    // -- BOTH --
    Task<GetScenarioNpcSkillDto?> AddNpcSkill(int npcId, CreateNpcSkillDto dto, int gameMasterId);
    Task<GetScenarioNpcSkillDto?> UpdateNpcSkill(UpdateNpcSkillDto dto, int gameMasterId);
    Task<bool> DeleteNpcSkill(int npcSkillId, int gameMasterId);
    
    // ================
    // -- COMPENDIUM --
    Task<GetScenarioNpcDto?> GetCompendiumNpc(int npcId);
    Task<IEnumerable<GetScenarioNpcDto>> GetCompendiumNpcs();
    Task<GetScenarioNpcDto?> CreateCompendiumNpc(CreateCompendiumNpcDto dto);
    Task<GetScenarioNpcDto?> UpdateCompendiumNpc(UpdateNpcDto dto);
    Task<bool> DeleteCompendiumNpc(int npcId);
}

public class NpcService : INpcService
{
    private readonly ApplicationDbContext _db;

    public NpcService(ApplicationDbContext db)
    {
        _db = db;
    }
    
    // ==============
    // -- CAMPAIGN --
    public async Task<GetScenarioNpcDto?> GetNpc(int npcId, int gameMasterId)
    {
        var npc = await IncludeAllNpcData()
            .FirstOrDefaultAsync(n => n.Id == npcId);
        if (npc == null || npc.Campaign == null || npc.Campaign.GameMasterId != gameMasterId) return null;

        return NpcDto(npc);
    }

    public async Task<IEnumerable<GetScenarioNpcDto>> GetNpcs(int campaignId, int gameMasterId)
    {
        var npcs = await IncludeAllNpcData()
            .Where(n => n.CampaignId == campaignId && n.Campaign != null &&  n.Campaign.GameMasterId == gameMasterId)
            .ToListAsync();

        return npcs.Select(NpcDto).ToList();
    }

    public async Task<GetScenarioNpcDto?> CreateNpc(CreateNpcDto dto, int gameMasterId)
    {
        var campaign = await _db.Campaigns
            .FirstOrDefaultAsync(c => c.Id == dto.CampaignId);
        if (campaign == null || campaign.GameMasterId != gameMasterId) return null;

        var npc = new Npc
        {
            CampaignId = dto.CampaignId,
            Name = dto.Name,
            Description = dto.Description,
            RaceId = dto.RaceId,
            ClassId = dto.ClassId,
            Level = dto.Level,
            Strength = dto.Strength,
            Dexterity = dto.Dexterity,
            Constitution = dto.Constitution,
            Intelligence = dto.Intelligence,
            Wisdom = dto.Wisdom,
            Charisma = dto.Charisma
        };

        _db.Npcs.Add(npc);
        await _db.SaveChangesAsync();

        return await GetNpc(npc.Id, gameMasterId);
    }

    public async Task<GetScenarioNpcDto?> UpdateNpc(UpdateNpcDto dto, int gameMasterId)
    {
        var npc = await IncludeAllNpcData()
            .FirstOrDefaultAsync(n => n.Id == dto.Id);
        if (npc == null || npc.Campaign == null || npc.Campaign.GameMasterId != gameMasterId) return null;

        npc.Name = dto.Name;
        npc.Description = dto.Description;
        npc.RaceId = dto.RaceId;
        npc.ClassId = dto.ClassId;
        npc.Level = dto.Level;
        npc.Strength = dto.Strength;
        npc.Dexterity = dto.Dexterity;
        npc.Constitution = dto.Constitution;
        npc.Intelligence = dto.Intelligence;
        npc.Wisdom = dto.Wisdom;
        npc.Charisma = dto.Charisma;

        await _db.SaveChangesAsync();

        return NpcDto(npc);
    }

    public async Task<bool> DeleteNpc(int npcId, int gameMasterId)
    {
        var npc = await _db.Npcs
            .Include(n => n.Campaign)
            .FirstOrDefaultAsync(n => n.Id == npcId);
        if (npc == null || npc.Campaign == null || npc.Campaign.GameMasterId != gameMasterId) return false;

        _db.Npcs.Remove(npc);
        await _db.SaveChangesAsync();

        return true;
    }
    
    
    // ==========
    // -- BOTH --
    public async Task<GetScenarioNpcSkillDto?> AddNpcSkill(int npcId, CreateNpcSkillDto dto, int gameMasterId)
    {
        var npc = await _db.Npcs
            .Include(n => n.Campaign)
            .FirstOrDefaultAsync(n => n.Id == npcId);
        if (npc == null || npc.Campaign == null || npc.Campaign.GameMasterId != gameMasterId) return null;

        var skill = new NpcSkill
        {
            NpcId = npcId,
            Name = dto.Name,
            Description = dto.Description,
            Value = dto.Value
        };

        _db.NpcSkills.Add(skill);
        await _db.SaveChangesAsync();

        return SkillDto(skill);
    }

    public async Task<GetScenarioNpcSkillDto?> UpdateNpcSkill(UpdateNpcSkillDto dto, int gameMasterId)
    {
        var skill = await _db.NpcSkills
            .Include(s => s.Npc)
                .ThenInclude(n => n.Campaign)
            .FirstOrDefaultAsync(s => s.Id == dto.Id);

        if (skill == null || skill.Npc.Campaign == null || skill.Npc.Campaign.GameMasterId != gameMasterId) return null;

        skill.Name = dto.Name;
        skill.Description = dto.Description;
        skill.Value = dto.Value;

        await _db.SaveChangesAsync();

        return SkillDto(skill);
    }

    public async Task<bool> DeleteNpcSkill(int npcSkillId, int gameMasterId)
    {
        var skill = await _db.NpcSkills
            .Include(s => s.Npc)
                .ThenInclude(n => n.Campaign)
            .FirstOrDefaultAsync(s => s.Id == npcSkillId);
        if (skill == null || skill.Npc.Campaign == null || skill.Npc.Campaign.GameMasterId != gameMasterId) return false;

        _db.NpcSkills.Remove(skill);
        await _db.SaveChangesAsync();

        return true;
    }

    
    // ================
    // -- COMPENDIUM --
    public async Task<GetScenarioNpcDto?> GetCompendiumNpc(int npcId)
    {
        var npc = await _db.Npcs
            .FirstOrDefaultAsync(n => n.Id == npcId && n.IsCompendium);
        if (npc == null) return null;

        return NpcDto(npc);
    }

    public async Task<IEnumerable<GetScenarioNpcDto>> GetCompendiumNpcs()
    {
        var npcs = await _db.Npcs
            .Where(n => n.IsCompendium)
            .ToListAsync();
        
        return npcs.Select(NpcDto).ToList();
    }

    public async Task<GetScenarioNpcDto?> CreateCompendiumNpc(CreateCompendiumNpcDto dto)
    {
        var npc = new Npc
        {
            CampaignId = null,
            Name = dto.Name,
            Description = dto.Description,
            RaceId = dto.RaceId,
            ClassId = dto.ClassId,
            Level = dto.Level,
            Strength = dto.Strength,
            Dexterity = dto.Dexterity,
            Constitution = dto.Constitution,
            Intelligence = dto.Intelligence,
            Wisdom = dto.Wisdom,
            Charisma = dto.Charisma
        };
        
        _db.Npcs.Add(npc);
        await _db.SaveChangesAsync();
        
        return NpcDto(npc);
    }

    public async Task<GetScenarioNpcDto?> UpdateCompendiumNpc(UpdateNpcDto dto)
    {
        var npc = await _db.Npcs
            .FirstOrDefaultAsync(n => n.Id == dto.Id &&  n.IsCompendium);
        if (npc == null) return null;
        
        npc.Name = dto.Name;
        npc.Description = dto.Description;
        npc.RaceId = dto.RaceId;
        npc.ClassId = dto.ClassId;
        npc.Level = dto.Level;
        npc.Strength = dto.Strength;
        npc.Dexterity = dto.Dexterity;
        npc.Constitution = dto.Constitution;
        npc.Intelligence = dto.Intelligence;
        npc.Wisdom = dto.Wisdom;
        npc.Charisma = dto.Charisma;
        
        await _db.SaveChangesAsync();
        
        return NpcDto(npc);
    }

    public async Task<bool> DeleteCompendiumNpc(int npcId)
    {
        var npc = await _db.Npcs
            .FirstOrDefaultAsync(n => n.Id == npcId &&  n.IsCompendium);
        if (npc == null) return false;
        
        _db.Npcs.Remove(npc);
        await _db.SaveChangesAsync();
        
        return true;
    }
    
    
    // ====================
    // -- HELPER METHODS --
    private IQueryable<Npc> IncludeAllNpcData()
    {
        return _db.Npcs
            .Include(n => n.Campaign)
            .Include(n => n.Race)
            .Include(n => n.Class)
            .Include(n => n.Skills);
    }

    private static GetScenarioNpcDto NpcDto(Npc npc)
    {
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
            Skills = npc.Skills.Select(SkillDto).ToList()
        };
    }

    private static GetScenarioNpcSkillDto SkillDto(NpcSkill skill)
    {
        return new GetScenarioNpcSkillDto
        {
            Id = skill.Id,
            NpcId = skill.NpcId,
            Name = skill.Name,
            Description = skill.Description,
            Value = skill.Value
        };
    }
}