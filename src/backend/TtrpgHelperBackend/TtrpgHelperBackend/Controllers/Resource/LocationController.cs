using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs.Resource.Location;
using TtrpgHelperBackend.Helpers;
using TtrpgHelperBackend.Services.Resource;

namespace TtrpgHelperBackend.Controllers.Resource;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LocationController : ControllerBase
{
    private readonly ILocationService _locationService;
    private readonly UserHelper _userHelper;

    public LocationController(ILocationService locationService, UserHelper userHelper)
    {
        _locationService = locationService;
        _userHelper = userHelper;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetScenarioLocationDto>> GetLocation(int id)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var location = await _locationService.GetLocation(id, gameMasterId.Value);
        if (location == null) return NotFound("Location not found.");

        return Ok(location);
    }

    [HttpGet("campaign/{campaignId}")]
    public async Task<ActionResult<IEnumerable<GetScenarioLocationDto>>> GetLocations(int campaignId)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var locations = await _locationService.GetLocations(campaignId, gameMasterId.Value);

        return Ok(locations);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateLocation([FromBody] CreateLocationDto dto)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var newLocation = await _locationService.CreateLocation(dto, gameMasterId.Value);
        if (newLocation == null) return BadRequest("You cannot create a new location in a campaign you do not own.");

        return Ok(newLocation);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateLocation(int id, [FromBody] UpdateLocationDto dto)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        if (id != dto.Id) return BadRequest("ID mismatch.");

        var updated = await _locationService.UpdateLocation(dto, gameMasterId.Value);
        if (updated == null) return NotFound("Location not found or not owned by GM.");

        return Ok(updated);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteLocation(int id)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var deleted = await _locationService.DeleteLocation(id, gameMasterId.Value);
        if (!deleted) return NotFound("Location not found or not owned by GM.");

        return Ok(deleted);
    }
}
