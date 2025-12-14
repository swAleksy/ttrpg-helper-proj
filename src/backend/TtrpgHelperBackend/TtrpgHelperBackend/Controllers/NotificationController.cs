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
    

    // GET: api/notification/unread
    [HttpGet("unread")]
    public async Task<IActionResult> GetUnreadNotifications()
    {
        var userId = _userHelper.GetUserId();
        var notifications = await _notificationService.GetUnreadNotificationsAsync(userId.Value);
        return Ok(notifications);
    }

// POST: api/notification/mark-read/{id}
    [HttpPost("mark-read/{id}")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var userId = _userHelper.GetUserId();
        await _notificationService.MarkAsReadAsync(id, userId.Value);
        return Ok(new { success = true });
    }

    // POST: api/notification/send
    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] SendNotificationRequestDto req)
    { 
        var fromUserId = _userHelper.GetUserId();
        var notification = await _notificationService.SendNotificationAsync(
            req.UserId, req.Type, req.Title, req.Message, fromUserId);

        return Ok(notification);
    }    
}
