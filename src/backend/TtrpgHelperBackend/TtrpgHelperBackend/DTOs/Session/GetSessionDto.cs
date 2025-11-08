using System.ComponentModel.DataAnnotations;

namespace TtrpgHelperBackend.DTOs.Session;

public class GetSessionDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime ScheduledDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public int GameMasterId { get; set; }

    public string GameMasterName { get; set; } = string.Empty;

    public IEnumerable<SessionPlayerDto> Players { get; set; } = new List<SessionPlayerDto>();
}

public class SessionPlayerDto
{
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
}