using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TtrpgHelperBackend.DTOs;
using TtrpgHelperBackend.Helpers;
using TtrpgHelperBackend.Services;

namespace TtrpgHelperBackend.Controllers;

[ApiController]
[Route("api/chat")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly UserHelper _userHelper;
    public ChatController(IChatService chatService, UserHelper userHelper)
    {
        _chatService = chatService;
        _userHelper = userHelper;
    }

    // Load private chat history
    [HttpGet("history/{otherUserId}")]
    public async Task<ActionResult<List<MessageDto>>> GetPrivateMessages(int otherUserId)
    {
        var userId = _userHelper.GetUserId(); 
        if (userId == null) return Unauthorized();

        // Konwersja int na string, bo ChatService operuje na stringach (zgodnie z Identity)
        var messages = await _chatService.GetPrivateChatHistory(userId.Value, otherUserId);
        
        return Ok(messages);
    }
}