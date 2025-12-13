using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs.Resource.Location;
using TtrpgHelperBackend.Services.Resource;

namespace TtrpgHelperBackend.Controllers.Resource;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CompendiumLocationController : ControllerBase
{
    private readonly ILocationService _locationService;

    public CompendiumLocationController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLocation(int id)
    {
        var location = await _locationService.GetCompendiumLocation(id);
        if (location == null) return NotFound("Location not found in compendium.");
        
        return Ok(location);
    }

    [HttpGet]
    public async Task<IActionResult> GetLocations()
    {
        var locations = await _locationService.GetCompendiumLocations();
        
        return Ok(locations);
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateLocation([FromBody] CreateCompendiumLocationDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var newLocation = await _locationService.CreateCompendiumLocation(dto);
        
        return Ok(newLocation);
    }
    
    [HttpPut("update/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateLocation(int id, [FromBody] UpdateLocationDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (id != dto.Id) return BadRequest("ID mismatch.");

        var updated = await _locationService.UpdateCompendiumLocation(dto);
        if (updated == null) return NotFound("Location not found in compendium.");

        return Ok(updated);
    }
    
    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteLocation(int id)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var deleted = await _locationService.DeleteCompendiumLocation(id);
        if (!deleted) return NotFound("Location not found in compendium.");

        return Ok("Compendium location deleted.");
    }
}