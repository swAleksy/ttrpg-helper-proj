using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TtrpgHelperBackend.MessagesAndNotofications;

namespace TtrpgHelperBackend.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly IHubContext<ChatHub> _hub;

    public TestController(IHubContext<ChatHub> hub)
    {
        _hub = hub;
    }

    // Send to a user (requires userId)
    [HttpPost("send-user")]
    public async Task<IActionResult> SendToUser([FromBody] SendDto dto)
    {
        await _hub.Clients.User(dto.UserId).SendAsync("ReceivePrivateMessage", "system", dto.Message);
        return Ok();
    }

    // Send to a group (session)
    [HttpPost("send-group")]
    public async Task<IActionResult> SendToGroup([FromBody] SendDto dto)
    {
        await _hub.Clients.Group(dto.GroupId).SendAsync("ReceiveTeamMessage", "system", dto.Message);
        return Ok();
    }
}

public class SendDto { public string UserId { get; set; } public string GroupId { get; set; } public string Message { get; set; } }
