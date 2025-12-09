using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs;
using TtrpgHelperBackend.Helpers;
using TtrpgHelperBackend.Services;

namespace TtrpgHelperBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class FriendController : ControllerBase
{
    private readonly IFriendService _friendService;
    private readonly UserHelper _userHelper;

    public FriendController(IFriendService friendService, UserHelper userHelper)
    {
        _friendService = friendService;
        _userHelper = userHelper;
    }

    [HttpPost("request/{targetId}")]
    public async Task<IActionResult> SendFriendRequest([FromRoute] int targetId)
    {
        var userId = _userHelper.GetUserId();
        if (userId == null) return Unauthorized("User ID not found in token.");
        
        var response = await _friendService.SendFriendRequestAsync(userId.Value, targetId);
        
        if (!response.Success)
            return BadRequest(response.Message);
            
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
        if (userId == null) return Unauthorized("User ID not found in token.");

        // Cleaned up logic: I am the CurrentUser, accepting a request from RequesterId
        var response = await _friendService.AcceptFriendRequestAsync(userId.Value, requesterId);
        
        if (!response.Success)
            return BadRequest(response.Message);
            
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