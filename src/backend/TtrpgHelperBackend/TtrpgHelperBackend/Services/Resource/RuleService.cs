using Microsoft.EntityFrameworkCore;
using TtrpgHelperBackend.DTOs.Resource.Rule;
using TtrpgHelperBackend.Models.Resource;

namespace TtrpgHelperBackend.Services.Resource;

public interface IRuleService
{
    Task<GetRuleDto?> GetRule(int ruleId);
    Task<IEnumerable<GetRuleDto>> GetRules();
    Task<GetRuleDto?> CreateRule(CreateRuleDto dto);
    Task<GetRuleDto?> UpdateRule(UpdateRuleDto dto);
    Task<bool> DeleteRule(int ruleId);
}

public class RuleService : IRuleService
{
    private readonly ApplicationDbContext _db;

    public RuleService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<GetRuleDto?> GetRule(int ruleId)
    {
        var rule = await _db.Rules
            .FirstOrDefaultAsync(r => r.Id == ruleId);
        if (rule == null) return null;

        return Dto(rule);
    }

    public async Task<IEnumerable<GetRuleDto>> GetRules()
    {
        var rules = await _db.Rules
            .ToListAsync();
        
        return rules.Select(r => Dto(r));
    }

    public async Task<GetRuleDto?> CreateRule(CreateRuleDto dto)
    {
        var rule = new Rule
        {
            Category = dto.Category,
            Name = dto.Name,
            ContentMarkdown = dto.ContentMarkdown,
        };
        
        _db.Rules.Add(rule);
        await _db.SaveChangesAsync();
        
        return Dto(rule);
    }

    public async Task<GetRuleDto?> UpdateRule(UpdateRuleDto dto)
    {
        var rule = await _db.Rules
            .FirstOrDefaultAsync(r => r.Id == dto.Id);
        if (rule == null) return null;
        
        rule.Category = dto.Category;
        rule.Name = dto.Name;
        rule.ContentMarkdown = dto.ContentMarkdown;
        
        await _db.SaveChangesAsync();
        
        return Dto(rule);
    }

    public async Task<bool> DeleteRule(int ruleId)
    {
        var rule = await _db.Rules
            .FirstOrDefaultAsync(r => r.Id == ruleId);
        if (rule == null) return false;
        
        _db.Rules.Remove(rule);
        await _db.SaveChangesAsync();
        
        return true;
    }

    
    // ====================
    // -- HELPER METHODS --
    private static GetRuleDto Dto(Rule rule)
    {
        return new GetRuleDto
        {
            Id = rule.Id,
            Category = rule.Category,
            Name = rule.Name,
            ContentMarkdown = rule.ContentMarkdown,
        };
    }
}
