using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs.Calendar;
using TtrpgHelperBackend.Helpers;
using TtrpgHelperBackend.Services.Calendar;

namespace TtrpgHelperBackend.Controllers.Calendar;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CalendarController : ControllerBase
{
    private readonly ICalendarService _calendarService;
    private readonly UserHelper _userHelper;

    public CalendarController(ICalendarService calendarService, UserHelper userHelper)
    {
        _calendarService = calendarService;
        _userHelper = userHelper;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCalendar()
    {
        var userId = _userHelper.GetUserId();
        if (userId == null) return Unauthorized("User ID not found in token.");

        var calendar = await _calendarService.GetCalendar(userId.Value);

        return Ok(calendar);
    }
    
    [HttpGet("event/{id}")]
    public async Task<IActionResult> GetEvent(int id)
    {
        var userId = _userHelper.GetUserId();
        if (userId == null) return Unauthorized("User ID not found in token.");

        var calendarEvent = await _calendarService.GetEvent(id, userId.Value);
        if (calendarEvent == null) return NotFound("Calendar event not found.");

        return Ok(calendarEvent);
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateEvent([FromBody] CreateCalendarEventDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = _userHelper.GetUserId();
        if (userId == null) return Unauthorized("User ID not found in token.");

        var newEvent = await _calendarService.CreateEvent(dto, userId.Value);

        return Ok(newEvent);
    }
    
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateCalendarEventDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (id != dto.Id) return BadRequest("ID mismatch.");

        var userId = _userHelper.GetUserId();
        if (userId == null) return Unauthorized("User ID not found in token.");

        var updated = await _calendarService.UpdateEvent(dto, userId.Value);
        if (updated == null) return NotFound("Calendar event not found or not owned by user.");

        return Ok(updated);
    }
    
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var userId = _userHelper.GetUserId();
        if (userId == null) return Unauthorized("User ID not found in token.");

        var deleted = await _calendarService.DeleteEvent(id, userId.Value);
        if (!deleted) return NotFound("Calendar event not found or not owned by user.");

        return Ok("Calendar event deleted.");
    }
}
