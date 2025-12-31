using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs.Resource.Background;
using TtrpgHelperBackend.Services.Resource;

namespace TtrpgHelperBackend.Controllers.Resource;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CompendiumBackgroundController : ControllerBase
{
    private readonly IBackgroundService _backgroundService;

    public CompendiumBackgroundController(IBackgroundService backgroundService)
    {
        _backgroundService = backgroundService;
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<GetBackgroundDto>> GetBackground(int id)
    {
        var background = await _backgroundService.GetBackground(id);
        if (background == null) return NotFound("Background not found in compendium.");
        
        return Ok(background);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetBackgroundDto>>> GetBackgrounds()
    {
        var backgrounds = await _backgroundService.GetBackgrounds();
        
        return Ok(backgrounds);
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CreateBackgroundDto>> CreateBackground([FromBody] CreateBackgroundDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var newBackground = await _backgroundService.CreateBackground(dto);
        
        return Ok(newBackground);
    }

    [HttpPut("update/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateBackground(int id, [FromBody] UpdateBackgroundDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        if (id != dto.Id) return BadRequest("ID mismatch.");
        
        var updated = await _backgroundService.UpdateBackground(dto);
        if (updated == null) return NotFound("Background not found in compendium.");
        
        return Ok(updated);
    }
    
    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBackground(int id)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var deleted = await _backgroundService.DeleteBackground(id);
        if (!deleted) return NotFound("Background not found in compendium.");

        return Ok("Compendium background deleted.");
    }
}
