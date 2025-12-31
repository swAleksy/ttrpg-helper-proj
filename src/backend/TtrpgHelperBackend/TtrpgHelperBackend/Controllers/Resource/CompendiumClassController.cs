using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs.Resource.Class;
using TtrpgHelperBackend.Services.Resource;

namespace TtrpgHelperBackend.Controllers.Resource;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CompendiumClassController : ControllerBase
{
    private readonly IClassService _classService;

    public CompendiumClassController(IClassService classService)
    {
        _classService = classService;
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<GetClassDto>> GetClass(int id)
    {
        var cls = await _classService.GetClass(id);
        if (cls == null) return NotFound("Class not found in compendium.");
        
        return Ok(cls);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetClassDto>>> GetClasses()
    {
        var classes = await _classService.GetClasses();
        
        return Ok(classes);
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateClass([FromBody] CreateClassDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var newClass = await _classService.CreateClass(dto);
        
        return Ok(newClass);
    }

    [HttpPut("update/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateClass(int id, [FromBody] UpdateClassDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        if (id != dto.Id) return BadRequest("ID mismatch.");
        
        var updated = await _classService.UpdateClass(dto);
        if (updated == null) return NotFound("Class not found in compendium.");
        
        return Ok(updated);
    }
    
    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteClass(int id)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var deleted = await _classService.DeleteClass(id);
        if (!deleted) return NotFound("Class not found in compendium.");

        return Ok("Compendium class deleted.");
    }
}
