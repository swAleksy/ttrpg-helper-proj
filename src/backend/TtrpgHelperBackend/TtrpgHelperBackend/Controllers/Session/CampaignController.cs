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

    // GET one specified campaign of GM 
    [HttpGet("{id}")]
    public async Task<ActionResult<GetCampaignDto?>> GetCampaign(int id)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");
        
        var campaign = await _campaignService.GetCampaign(id);
        if (campaign == null) return NotFound("Campaign not found.");
        
        return Ok(campaign);
    }
    
    // GET all campaigns of GM
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetCampaignDto>>> GetCampaigns()
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");
        
        var campaigns = await _campaignService.GetCampaigns();
        
        return Ok(campaigns);
    }
    
    // POST
    [HttpPost("create")]
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
    
    // DELETE
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteCampaign(int id)
    {
        var deleted = await _campaignService.DeleteCampaign(id);
        if (!deleted) return NotFound("Campaign not found.");
            
        return Ok("Campaign deleted successfully.");
    }
    
    // POST
    [HttpPost("archive/{id}")]
    public async Task<IActionResult> ArchiveCampaign(int id)
    {
        if (!await _campaignService.ArchiveCampaign(id)) return NotFound("Campaign not found.");
        
        return Ok("Campaign archived successfully.");
    }
}
