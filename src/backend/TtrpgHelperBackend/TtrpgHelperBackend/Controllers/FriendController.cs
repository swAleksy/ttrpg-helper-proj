using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs;
using TtrpgHelperBackend.Helpers;
using TtrpgHelperBackend.Models;
using TtrpgHelperBackend.Services;

namespace TtrpgHelperBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class FriendController : ControllerBase
{
    private readonly IFriendService _friendService;
    private readonly INotificationService _notificationService; // Zamiast IHubContext
    private readonly UserHelper _userHelper;

    public FriendController(IFriendService friendService, INotificationService notificationService, UserHelper userHelper)
    {
        _friendService = friendService;
        _notificationService = notificationService;
        _userHelper = userHelper;
    }

    [HttpPost("request/{targetId}")]
    public async Task<IActionResult> SendFriendRequest([FromRoute] int targetId)
    {
        var userId = _userHelper.GetUserId();
        if (userId == null) return Unauthorized();

        var response = await _friendService.SendFriendRequestAsync(userId.Value, targetId);

        if (!response.Success)
            return BadRequest(response.Message);

        // Używamy serwisu powiadomień -> Zapisze w DB i wyśle SignalR
        await _notificationService.SendNotification(
            targetId, 
            NotificationType.FriendRequest, 
            "Nowe zaproszenie", 
            $"Użytkownik {_userHelper.GetUserName()} wysłał Ci zaproszenie.", 
            userId.Value
        );

        return Ok(response.Message);
    }

    [HttpDelete("{targetId}")]
    public async Task<IActionResult> RemoveFriend([FromRoute] int targetId)
    {
        var userId = _userHelper.GetUserId();
        if (userId == null) return Unauthorized("User ID not found in token.");

        var response = await _friendService.RemoveFriendshipAsync(userId.Value, targetId);
        
        if (!response.Success)
            return BadRequest(response.Message);
            
        return Ok(response.Message);
    }
    
    [HttpPut("accept/{requesterId}")]
    public async Task<IActionResult> AcceptFriendRequest([FromRoute] int requesterId)
    {
        var userId = _userHelper.GetUserId();
        if (userId == null) return Unauthorized();

        var response = await _friendService.AcceptFriendRequestAsync(userId.Value, requesterId);

        if (!response.Success)
            return BadRequest(response.Message);

        // Powiadom osobę, która wysłała zaproszenie, że zostało przyjęte
        await _notificationService.SendNotification(
            requesterId, 
            NotificationType.FriendRequestAccepted, 
            "Zaproszenie przyjęte", 
            $"Użytkownik {_userHelper.GetUserName()} zaakceptował Twoje zaproszenie.", 
            userId.Value
        );

        return Ok(response.Message);
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<UserInfoDto>>> GetFriends()
    {
        var userId = _userHelper.GetUserId();
        if (userId == null) return Unauthorized("User ID not found in token.");

        var friendsList = await _friendService.GetFriendsAsync(userId.Value);
        return Ok(friendsList);
    }

    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<UserInfoDto>>> GetPendingRequests()
    {
        var userId = _userHelper.GetUserId();
        if (userId == null) return Unauthorized("User ID not found in token.");

        var requests = await _friendService.GetPendingRequestsAsync(userId.Value);
        return Ok(requests);
    }
}