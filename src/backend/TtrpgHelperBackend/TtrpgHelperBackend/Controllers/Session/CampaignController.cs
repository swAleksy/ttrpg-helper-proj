using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs.Session;
using TtrpgHelperBackend.Services.Session;
using TtrpgHelperBackend.Helpers;

namespace TtrpgHelperBackend.Controllers.Session;

[ApiController]
[Route("api/[controller]")]
public class CampaignController : ControllerBase
{
    private readonly ICampaignService _campaignService;
    private readonly ApplicationDbContext _context;
    private readonly UserHelper _userHelper;

    public CampaignController(ICampaignService campaignService,  ApplicationDbContext context, UserHelper userHelper)
    {
        _campaignService = campaignService;
        _context = context;
        _userHelper = userHelper;
    }

    // GET one specified campaign of GM 
    [Authorize]
    [HttpGet("gm/{id}/{gameMasterId}")]
    public async Task<ActionResult<GetCampaignDto?>> GetCampaign(int id, int gameMasterId)
    {
        var campaign = await _campaignService.GetCampaign(id, gameMasterId);
        if (campaign == null) return NotFound();
        
        return Ok(campaign);
    }
    
    // GET all campaigns of GM
    [Authorize]
    [HttpGet("gm/{gameMasterId}")]
    public async Task<ActionResult<IEnumerable<GetCampaignDto>>> GetCampaigns(int gameMasterId)
    {
        var campaigns = await _campaignService.GetCampaigns(gameMasterId);
        
        return Ok(campaigns);
    }
    
    // POST
    [Authorize]
    [HttpPost]
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
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCampaign(int id)
    {
        var deleted = await _campaignService.DeleteCampaign(id);
        if (!deleted) return NotFound("Campaign not found.");
            
        return Ok("Campaign deleted successfully.");
    }
}
