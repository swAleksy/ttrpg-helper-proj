using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs;
using TtrpgHelperBackend.Helpers;
using TtrpgHelperBackend.Services;

namespace TtrpgHelperBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly UserHelper _userHelper;
    public NotificationController(INotificationService notificationService,  UserHelper userHelper)
    {
        _notificationService = notificationService;
        _userHelper = userHelper;
    }
    

    [HttpGet("unread")]
    public async Task<ActionResult<List<NotificationDto>>> GetUnreadNotifications()
    {
        var userId = _userHelper.GetUserId();
        if (userId == null)
            return Unauthorized();
        
        return Ok(await _notificationService.GetUnreadNotificationsAsync(userId.Value));
    }

    [HttpPost("mark-read/{id}")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var userId = _userHelper.GetUserId();
        if (userId == null)
            return Unauthorized();
        
        await _notificationService.MarkAsReadAsync(id, userId.Value);
        return Ok();
    }
}
