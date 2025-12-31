using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs.Resource.Race;
using TtrpgHelperBackend.Services.Resource;

namespace TtrpgHelperBackend.Controllers.Resource;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CompendiumRaceController : ControllerBase
{
    private readonly IRaceService _raceService;

    public CompendiumRaceController(IRaceService raceService)
    {
        _raceService = raceService;
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<GetRaceDto>> GetRace(int id)
    {
        var race = await _raceService.GetRace(id);
        if (race == null) return NotFound("Race not found in compendium.");
        
        return Ok(race);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetRaceDto>>> GetRaces()
    {
        var races = await _raceService.GetRaces();
        
        return Ok(races);
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateRace([FromBody] CreateRaceDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var newRace = await _raceService.CreateRace(dto);
        
        return Ok(newRace);
    }

    [HttpPut("update/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRace(int id, [FromBody] UpdateRaceDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        if (id != dto.Id) return BadRequest("ID mismatch.");
        
        var updated = await _raceService.UpdateRace(dto);
        if (updated == null) return NotFound("Race not found in compendium.");
        
        return Ok(updated);
    }
    
    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRace(int id)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var deleted = await _raceService.DeleteRace(id);
        if (!deleted) return NotFound("Race not found in compendium.");

        return Ok("Compendium race deleted.");
    }
}
