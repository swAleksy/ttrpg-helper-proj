using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.Services;

namespace TtrpgHelperBackend.Controllers;

[ApiController]
[Route("api/chat")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly ChatService _chatService;
    public ChatController(ChatService chatService)
    {
        _chatService = chatService;
    }

    // Load private chat history
    [HttpGet("private/{otherUserId}")]
    public async Task<IActionResult> GetPrivateMessages(string otherUserId)
    {
        var userId = GetUserId();
        var messages = await _chatService.GetPrivateChatHistory(userId, otherUserId);
        return Ok(messages);
    }

    // Load team chat history
    [HttpGet("session/{sessionId}")]
    public async Task<IActionResult> GetSessionMessages(string sessionId)
    {
        var messages = await _chatService.GetSessionChatHistory(sessionId);
        return Ok(messages);
    }
    
    private string GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
            throw new Exception("User ID claim (NameIdentifier) is missing from the principal.");
        return userId;
    }
}