using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs.Resource;
using TtrpgHelperBackend.Helpers;
using TtrpgHelperBackend.Services.Resource;

namespace TtrpgHelperBackend.Controllers.Resource;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ScenarioChapterController : ControllerBase
{
    private readonly IScenarioChapterService _scenarioChapterService;
    private readonly UserHelper _userHelper;

    public ScenarioChapterController(IScenarioChapterService scenarioChapterService, UserHelper userHelper)
    {
        _scenarioChapterService = scenarioChapterService;
        _userHelper = userHelper;
    }

    // ======================
    // -- SCENARIO CHAPTER --
    [HttpGet("{id}")]
    public async Task<IActionResult> GetScenarioChapter(int id)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var chapter = await _scenarioChapterService.GetScenarioChapter(id, gameMasterId.Value);
        if (chapter == null) return NotFound("Chapter not found.");

        return Ok(chapter);
    }

    [HttpGet("scenario/{scenarioId}")]
    public async Task<IActionResult> GetScenario(int scenarioId)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var chapters = await _scenarioChapterService.GetScenarioChapters(scenarioId, gameMasterId.Value);

        return Ok(chapters);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateScenarioChapter([FromBody] CreateScenarioChapterDto dto)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var newChapter = await _scenarioChapterService.CreateScenarioChapter(dto, gameMasterId.Value);
        if (newChapter == null) return BadRequest("You cannot create a new chapter in a scenario you do not own.");

        return Ok(newChapter);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateScenario(int id, [FromBody] UpdateScenarioChapterDto dto)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        if (id != dto.Id) return BadRequest("ID mismatch.");

        var updated = await _scenarioChapterService.UpdateScenarioChapter(dto, gameMasterId.Value);
        if (updated == null) return NotFound("Chapter not found or not owned by GM.");

        return Ok(updated);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteScenario(int id)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var deleted = await _scenarioChapterService.DeleteScenarioChapter(id, gameMasterId.Value);
        if (!deleted) return NotFound("Chapter not found or not owned by GM.");

        return Ok(deleted);
    }
    // -- SCENARIO CHAPTER --
    // ======================
    

    // ==========
    // -- NOTE --
    [HttpPost("append/{chapterId}/note/{noteId}")]
    public async Task<IActionResult> AddNoteToChapter(int chapterId, int noteId)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");
        
        var note = await _scenarioChapterService.AddNoteToChapter(chapterId, noteId, gameMasterId.Value);
        if (note == null) return BadRequest("Note cannot be added to this chapter.");
        
        return Ok(note);
    }

    [HttpDelete("remove/{chapterId}/note/{noteId}")]
    public async Task<IActionResult> DeleteNoteFromChapter(int chapterId, int noteId)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");
        
        var deleted = await _scenarioChapterService.DeleteNoteFromChapter(chapterId, noteId, gameMasterId.Value);
        if (!deleted) return NotFound("Note not found or not owned by GM.");
        
        return Ok(deleted);
    }
    

    // ==========
    // -- ITEM --
    [HttpPost("append/{chapterId}/item/{itemId}")]
    public async Task<IActionResult> AddItemToChapter(int chapterId, int itemId)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var result = await _scenarioChapterService.AddItemToChapter(chapterId, itemId, gameMasterId.Value);
        if (result == null) return BadRequest("Item cannot be added to this chapter.");

        return Ok(result);
    }

    [HttpDelete("remove/{chapterId}/item/{itemId}")]
    public async Task<IActionResult> RemoveItemFromChapter(int chapterId, int itemId)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var deleted = await _scenarioChapterService.DeleteItemFromChapter(chapterId, itemId, gameMasterId.Value);
        if (!deleted) return NotFound("Item not found or not owned by GM.");

        return Ok(deleted);
    }
    

    // ==============
    // -- LOCATION --
    [HttpPost("append/{chapterId}/location/{locationId}")]
    public async Task<IActionResult> AddLocationToChapter(int chapterId, int locationId)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var result = await _scenarioChapterService.AddLocationToChapter(chapterId, locationId, gameMasterId.Value);
        if (result == null) return BadRequest("Location cannot be added to this chapter.");

        return Ok(result);
    }

    [HttpDelete("remove/{chapterId}/location/{locationId}")]
    public async Task<IActionResult> RemoveLocationFromChapter(int chapterId, int locationId)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var deleted = await _scenarioChapterService.DeleteLocationFromChapter(chapterId, locationId, gameMasterId.Value);
        if (!deleted) return NotFound("Location not found or not owned by GM.");

        return Ok(deleted);
    }
    

    // =========
    // -- NPC --
    [HttpPost("append/{chapterId}/npc/{npcId}")]
    public async Task<IActionResult> AddNpcToChapter(int chapterId, int npcId)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var result = await _scenarioChapterService.AddNpcToChapter(chapterId, npcId, gameMasterId.Value);
        if (result == null) return BadRequest("NPC cannot be added to this chapter.");

        return Ok(result);
    }

    [HttpDelete("remove/{chapterId}/npc/{npcId}")]
    public async Task<IActionResult> RemoveNpcFromChapter(int chapterId, int npcId)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");

        var deleted = await _scenarioChapterService.DeleteNpcFromChapter(chapterId, npcId, gameMasterId.Value);
        if (!deleted) return NotFound("NPC not found or not owned by GM.");

        return Ok(deleted);
    }
}
