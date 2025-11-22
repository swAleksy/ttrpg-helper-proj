using TtrpgHelperBackend.Models;

namespace TtrpgHelperBackend.DTOs;

public class SendNotificationRequestDto
{
    public string UserId { get; set; } = default!;
    public NotificationType Type { get; set; }
    public string Title { get; set; } = default!;
    public string Message { get; set; } = default!;
}