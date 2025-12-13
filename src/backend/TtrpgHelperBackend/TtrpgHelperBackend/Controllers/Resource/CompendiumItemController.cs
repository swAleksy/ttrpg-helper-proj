using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs.Resource.Item;
using TtrpgHelperBackend.Services.Resource;

namespace TtrpgHelperBackend.Controllers.Resource;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CompendiumItemController : ControllerBase
{
    private readonly IItemService _itemService;

    public CompendiumItemController(IItemService itemService)
    {
        _itemService = itemService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetItem(int id)
    {
        var item = await _itemService.GetCompendiumItem(id);
        if (item == null) return NotFound("Item not found in compendium.");
        
        return Ok(item);
    }

    [HttpGet]
    public async Task<IActionResult> GetItems()
    {
        var items = await _itemService.GetCompendiumItems();
        
        return Ok(items);
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateItem([FromBody] CreateCompendiumItemDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var newItem = await _itemService.CreateCompendiumItem(dto);
        
        return Ok(newItem);
    }

    [HttpPut("update/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateItem(int id, [FromBody] UpdateItemDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        if (id != dto.Id) return BadRequest("ID mismatch.");
        
        var updated = await _itemService.UpdateCompendiumItem(dto);
        if (updated == null) return NotFound("Item not found in compendium.");
        
        return Ok(updated);
    }
    
    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var deleted = await _itemService.DeleteCompendiumItem(id);
        if (!deleted) return NotFound("Item not found in compendium.");

        return Ok("Compendium item deleted.");
    }
}
