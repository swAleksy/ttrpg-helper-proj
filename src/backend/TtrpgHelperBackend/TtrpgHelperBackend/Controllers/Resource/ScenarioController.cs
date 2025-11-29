using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs.Resource;
using TtrpgHelperBackend.Helpers;
using TtrpgHelperBackend.Models.Resource;
using TtrpgHelperBackend.Services.Resource;

namespace TtrpgHelperBackend.Controllers.Resource;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ScenarioController : ControllerBase
{
    private readonly IScenarioService _scenarioService;
    private readonly UserHelper _userHelper;

    public ScenarioController(IScenarioService scenarioService, UserHelper userHelper)
    {
        _scenarioService = scenarioService;
        _userHelper = userHelper;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetScenario(int id)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");
        
        var scenario = await _scenarioService.GetScenario(id,  gameMasterId.Value);
        if (scenario == null) return NotFound("Scenario not found.");
        
        return Ok(scenario);
    }

    [HttpGet("campaign/{campaignId}")]
    public async Task<IActionResult> GetScenarios(int campaignId)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");
        
        var scenarios = await _scenarioService.GetScenarios(campaignId, gameMasterId.Value);
        
        return Ok(scenarios);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateScenario([FromBody] CreateScenarioDto dto)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");
        
        var newScenario = await _scenarioService.CreateScenario(dto, gameMasterId.Value);
        if (newScenario == null) return BadRequest("You cannot create a new scenario in a campaign you do not own.");
        
        return Ok(newScenario);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateScenario(int id, [FromBody] UpdateScenarioDto dto)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");
        
        if (id != dto.Id) return BadRequest("ID mismatch.");
        
        var updated = await _scenarioService.UpdateScenario(dto, gameMasterId.Value);
        if (updated == null) return NotFound("Scenario not found or not owned by GM.");
        
        return Ok(updated);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteScenario(int id)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");
        
        var deleted = await _scenarioService.DeleteScenario(id, gameMasterId.Value);
        if (!deleted) return NotFound("Scenario not found or not owned by GM.");
        
        return Ok(deleted);
    }
}
