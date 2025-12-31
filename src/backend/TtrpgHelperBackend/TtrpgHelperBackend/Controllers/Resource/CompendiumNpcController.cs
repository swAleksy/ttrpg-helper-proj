using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs.Resource.Npc;
using TtrpgHelperBackend.Services.Resource;

namespace TtrpgHelperBackend.Controllers.Resource;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CompendiumNpcController : ControllerBase
{
    private readonly INpcService _npcService;

    public CompendiumNpcController(INpcService npcService)
    {
        _npcService = npcService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetScenarioNpcDto>> GetNpc(int id)
    {
        var npc = await _npcService.GetCompendiumNpc(id);
        if (npc == null) return NotFound("NPC not found in compendium.");

        return Ok(npc);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetScenarioNpcDto>>> GetNpcs()
    {
        var npcs = await _npcService.GetCompendiumNpcs();
        
        return Ok(npcs);
    }
    
    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateNpc([FromBody] CreateCompendiumNpcDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var newNpc = await _npcService.CreateCompendiumNpc(dto);
        
        return Ok(newNpc);
    }

    [HttpPut("update/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateNpc(int id, [FromBody] UpdateNpcDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (id != dto.Id) return BadRequest("ID mismatch.");

        var updated = await _npcService.UpdateCompendiumNpc(dto);
        if (updated == null) return NotFound("NPC not found in compendium.");

        return Ok(updated);
    }

    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteNpc(int id)
    {
        var deleted = await _npcService.DeleteCompendiumNpc(id);
        if (!deleted) return NotFound("NPC not found in compendium.");

        return Ok("Compendium NPC deleted.");
    }
}
