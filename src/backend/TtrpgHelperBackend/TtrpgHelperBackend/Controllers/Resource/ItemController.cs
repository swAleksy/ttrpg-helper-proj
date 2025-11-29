using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs.Resource;
using TtrpgHelperBackend.Helpers;
using TtrpgHelperBackend.Services.Resource;

namespace TtrpgHelperBackend.Controllers.Resource;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ItemController : ControllerBase
{
    private readonly IItemService _itemService;
    private readonly UserHelper _userHelper;

    public ItemController(IItemService itemService, UserHelper userHelper)
    {
        _itemService = itemService;
        _userHelper = userHelper;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetItem(int id)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var item = await _itemService.GetItem(id, gameMasterId.Value);
        if (item == null) return NotFound("Item not found.");

        return Ok(item);
    }
    
    [HttpGet("campaign/{campaignId}")]
    public async Task<IActionResult> GetItems(int campaignId)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var items = await _itemService.GetItems(campaignId, gameMasterId.Value);
        
        return Ok(items);
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateItem([FromBody] CreateItemDto dto)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var newItem = await _itemService.CreateItem(dto, gameMasterId.Value);
        if (newItem == null) return BadRequest("You cannot create a new item in a campaign you do not own.");

        return Ok(newItem);
    }
    
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateItem(int id, [FromBody] UpdateItemDto dto)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        if (id != dto.Id) return BadRequest("ID mismatch.");

        var updated = await _itemService.UpdateItem(dto, gameMasterId.Value);
        if (updated == null) return NotFound("Item not found or not owned by GM.");

        return Ok(updated);
    }
    
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var deleted = await _itemService.DeleteItem(id, gameMasterId.Value);
        if (!deleted) return NotFound("Item not found or not owned by GM.");

        return Ok(deleted);
    }
}
