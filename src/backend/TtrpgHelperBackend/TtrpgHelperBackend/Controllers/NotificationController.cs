using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs;
using TtrpgHelperBackend.Services;

namespace TtrpgHelperBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }
    

    // GET: api/notification/unread
    [HttpGet("unread")]
    public async Task<IActionResult> GetUnreadNotifications()
    {
        var userId = GetUserId();
        var notifications = await _notificationService.GetUnreadNotificationsAsync(userId);
        return Ok(notifications);
    }

    // POST: api/notification/read/5
    [HttpPost("read/{id}")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var userId = GetUserId();
        await _notificationService.MarkAsReadAsync(id, userId);
        return Ok(new { success = true });
    }

    // POST: api/notification/send
    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] SendNotificationRequestDto req)
    {
        var fromUserId = GetUserId();
        var notification = await _notificationService.SendNotificationAsync(
            req.UserId, req.Type, req.Title, req.Message, fromUserId);

        return Ok(notification);
    }    
    
    private string GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
            throw new Exception("User ID claim (NameIdentifier) is missing from the principal.");
        return userId;
    }
}
