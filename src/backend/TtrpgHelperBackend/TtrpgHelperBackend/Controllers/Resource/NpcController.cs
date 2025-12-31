using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs.Resource.Npc;
using TtrpgHelperBackend.Helpers;
using TtrpgHelperBackend.Services.Resource;

namespace TtrpgHelperBackend.Controllers.Resource;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NpcController : ControllerBase
{
    private readonly INpcService _npcService;
    private readonly UserHelper _userHelper;

    public NpcController(INpcService npcService, UserHelper userHelper)
    {
        _npcService = npcService;
        _userHelper = userHelper;
    }

    // =========
    // -- NPC --
    [HttpGet("{id}")]
    public async Task<ActionResult<GetScenarioNpcDto>> GetNpc(int id)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var npc = await _npcService.GetNpc(id, gameMasterId.Value);
        if (npc == null) return NotFound("NPC not found.");

        return Ok(npc);
    }

    [HttpGet("campaign/{campaignId}")]
    public async Task<ActionResult<IEnumerable<GetScenarioNpcDto>>> GetNpcs(int campaignId)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var npcs = await _npcService.GetNpcs(campaignId, gameMasterId.Value);

        return Ok(npcs);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateNpc([FromBody] CreateNpcDto dto)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var npc = await _npcService.CreateNpc(dto, gameMasterId.Value);
        if (npc == null) return BadRequest("You cannot create a new NPC in a campaign you do not own.");

        return Ok(npc);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateNpc(int id, [FromBody] UpdateNpcDto dto)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        if (id != dto.Id) return BadRequest("ID mismatch.");

        var npc = await _npcService.UpdateNpc(dto, gameMasterId.Value);
        if (npc == null) return NotFound("NPC not found or not owned by GM.");

        return Ok(npc);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteNpc(int id)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var deleted = await _npcService.DeleteNpc(id, gameMasterId.Value);
        if (!deleted) return NotFound("NPC not found or not owned by GM.");

        return Ok(deleted);
    }
    // -- NPC --
    // =========


    // =================
    // -- NPC SKILLS --
    [HttpPost("add/{npcId}/skill")]
    public async Task<IActionResult> AddNpcSkill(int npcId, [FromBody] CreateNpcSkillDto dto)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var skill = await _npcService.AddNpcSkill(npcId, dto, gameMasterId.Value);
        if (skill == null) return BadRequest("Skill cannot be added to NPC.");

        return Ok(skill);
    }

    [HttpPut("update/skill/{skillId}")]
    public async Task<IActionResult> UpdateNpcSkill(int skillId, [FromBody] UpdateNpcSkillDto dto)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        if (skillId != dto.Id) return BadRequest("ID mismatch.");

        var skill = await _npcService.UpdateNpcSkill(dto, gameMasterId.Value);
        if (skill == null) return NotFound("NPC skill not found or not owned by GM.");

        return Ok(skill);
    }

    [HttpDelete("delete/skill/{skillId}")]
    public async Task<IActionResult> DeleteNpcSkill(int skillId)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var deleted = await _npcService.DeleteNpcSkill(skillId, gameMasterId.Value);
        if (!deleted) return NotFound("NPC skill not found or not owned by GM.");

        return Ok(deleted);
    }
}
