using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs.Resource.Rule;
using TtrpgHelperBackend.Services.Resource;

namespace TtrpgHelperBackend.Controllers.Resource;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CompendiumRuleController : ControllerBase
{
    private readonly IRuleService _ruleService;

    public CompendiumRuleController(IRuleService ruleService)
    {
        _ruleService = ruleService;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRule(int id)
    {
        var rule = await _ruleService.GetRule(id);
        if (rule == null) return NotFound("Rule not found in compendium.");
        
        return Ok(rule);
    }

    [HttpGet]
    public async Task<IActionResult> GetRules()
    {
        var rules = await _ruleService.GetRules();
        
        return Ok(rules);
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateRule([FromBody] CreateRuleDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var newRule = await _ruleService.CreateRule(dto);
        
        return Ok(newRule);
    }

    [HttpPut("update/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRule(int id, [FromBody] UpdateRuleDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        if (id != dto.Id) return BadRequest("ID mismatch.");
        
        var updated = await _ruleService.UpdateRule(dto);
        if (updated == null) return BadRequest("Rule not found in compendium.");
        
        return Ok(updated);
    }

    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRule(int id)
    {
        var deleted = await _ruleService.DeleteRule(id);
        if (!deleted) return NotFound("Rule not found in compendium.");
        
        return Ok(deleted);
    }
}
