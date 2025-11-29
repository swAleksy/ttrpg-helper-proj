using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs.Resource;
using TtrpgHelperBackend.Helpers;
using TtrpgHelperBackend.Services.Resource;

namespace TtrpgHelperBackend.Controllers.Resource;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NoteController : ControllerBase
{
    private readonly INoteService _noteService;
    private readonly UserHelper _userHelper;

    public NoteController(INoteService noteService, UserHelper userHelper)
    {
        _noteService = noteService;
        _userHelper = userHelper;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNote(int id)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");
        
        var note = await _noteService.GetNote(id, gameMasterId.Value);
        if (note == null) return NotFound("Note not found.");
        
        return Ok(note);
    }

    [HttpGet("campaign/{campaignId}")]
    public async Task<IActionResult> GetNotes(int campaignId)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");
        
        var notes = await _noteService.GetNotes(campaignId, gameMasterId.Value);
        
        return Ok(notes);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateNote([FromBody] CreateNoteDto dto)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");
        
        var newNote = await _noteService.CreateNote(dto, gameMasterId.Value);
        if (newNote == null) return NotFound("You cannot create a new note in a campaign you do not own.");
        
        return  Ok(newNote);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateNote(int id, [FromBody] UpdateNoteDto dto)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");
        
        if (id != dto.Id) return BadRequest("ID mismatch.");
        
        var updated = await _noteService.UpdateNote(dto, gameMasterId.Value);
        if (updated == null) return NotFound("Note not found or not owned by GM.");
        
        return Ok(updated);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteNote(int id)
    {
        var gameMasterId = _userHelper.GetUserId();
        if (gameMasterId == null) return Unauthorized("User ID not found in token.");
        
        var deleted = await _noteService.DeleteNote(id, gameMasterId.Value);
        if (!deleted) return NotFound("Note not found or not owned by GM.");
        
        return Ok(deleted);
    }
}
