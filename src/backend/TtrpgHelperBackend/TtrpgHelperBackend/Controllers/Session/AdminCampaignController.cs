using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.Services.Session;

namespace TtrpgHelperBackend.Controllers.Session;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminCampaignController : ControllerBase
{
    private readonly ICampaignService _campaignService;

    public AdminCampaignController(ICampaignService campaignService)
    {
        _campaignService = campaignService;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var campaign = await _campaignService.GetCampaignForAdmin(id);
        if (campaign == null) return NotFound("Campaign not found.");

        return Ok(campaign);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status)
    {
        var campaigns = await _campaignService.GetCampaignsForAdmin(status);
        
        return Ok(campaigns);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _campaignService.DeleteCampaign(id);
        if (!deleted) return NotFound("Campaign not found.");

        return Ok("Campaign deleted successfully.");
    }
    
    [HttpPost("archive/{id}")]
    public async Task<IActionResult> Archive(int id)
    {
        var archived = await _campaignService.ArchiveCampaign(id);
        if (!archived) return NotFound("Campaign not found.");

        return Ok("Campaign archived successfully.");
    }
}
