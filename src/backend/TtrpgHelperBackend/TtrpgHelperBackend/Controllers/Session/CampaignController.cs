using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs.Session;
using TtrpgHelperBackend.Services.Session;
using TtrpgHelperBackend.Helpers;

namespace TtrpgHelperBackend.Controllers.Session;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CampaignController : ControllerBase
{
    private readonly ICampaignService _campaignService;
    private readonly UserHelper _userHelper;

    public CampaignController(ICampaignService campaignService, UserHelper userHelper)
    {
        _campaignService = campaignService;
        _userHelper = userHelper;
    }
    
    // -- GAMEMASTER --
    // GET one specified campaign of GM 
    [HttpGet("gm/{id}")]
    public async Task<ActionResult<GetCampaignDto?>> GetCampaignForGm(int id)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");
        
        var campaign = await _campaignService.GetCampaignForGm(id, gameMasterId.Value);
        if (campaign == null) return NotFound("Campaign not found.");
        
        return Ok(campaign);
    }
    
    // GET all campaigns of GM, optional status filtering
    [HttpGet("gm")]
    public async Task<ActionResult<IEnumerable<GetCampaignDto>>> GetCampaignsForGm([FromQuery] string? status)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");
        
        var campaigns = await _campaignService.GetCampaignsForGm(gameMasterId.Value, status);
        
        return Ok(campaigns);
    }
    // -- GAMEMASTER --
    
    // -- PLAYER --
    // GET one specified campaign of PLAYER 
    [HttpGet("player/{id}")]
    public async Task<ActionResult<GetCampaignDto?>> GetCampaignForPlayer(int id)
    {
        var playerId = _userHelper.GetUserId();
        if (playerId == null) return Unauthorized("User ID not found in token.");
        
        var campaign = await _campaignService.GetCampaignForPlayer(id, playerId.Value);
        if (campaign == null) return NotFound("Campaign not found.");
        
        return Ok(campaign);
    }
    
    // GET all campaigns of PLAYER, optional status filtering
    [HttpGet("player")]
    public async Task<ActionResult<IEnumerable<GetCampaignDto>>> GetCampaignsForPlayer([FromQuery] string? status)
    {
        var playerId = _userHelper.GetUserId();
        if (playerId == null) return Unauthorized("User ID not found in token.");
        
        var campaigns = await _campaignService.GetCampaignsForPlayer(playerId.Value, status);
        
        return Ok(campaigns);
    }
    // -- PLAYER --
    
    // CREATE (GM only)
    [HttpPost("gm/create")]
    public async Task<ActionResult<GetCampaignDto>> CreateCampaign([FromBody] CreateCampaignDto dto)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");
    
        var newCampaign = await _campaignService.CreateCampaign(new CreateCampaignDto
        {
            Name = dto.Name,
            Description = dto.Description,
            GameMasterId = gameMasterId.Value
        });
        
        return Ok(newCampaign); 
    }
    
    // DELETE (GM only)
    [HttpDelete("gm/delete/{id}")]
    public async Task<IActionResult> DeleteCampaign(int id)
    {
        var gmId = _userHelper.GetUserId();
        if (gmId == null) return Unauthorized();

        var campaign = await _campaignService.GetCampaignForGm(id, gmId.Value);
        if (campaign == null) return Forbid("You cannot delete a campaign you do not own.");

        var deleted = await _campaignService.DeleteCampaign(id);
        if (!deleted) return NotFound("Campaign not found.");

        return Ok("Campaign deleted successfully.");
    }
    
    // ARCHIVE (GM only)
    [HttpPost("gm/archive/{id}")]
    public async Task<IActionResult> ArchiveCampaign(int id)
    {
        var gmId = _userHelper.GetUserId();
        if (gmId == null) return Unauthorized();

        var campaign = await _campaignService.GetCampaignForGm(id, gmId.Value);
        if (campaign == null) return Forbid("You cannot archive a campaign you do not own.");

        var archived = await _campaignService.ArchiveCampaign(id);
        if (!archived) return NotFound("Campaign not found.");

        return Ok("Campaign archived successfully.");
    }
}
