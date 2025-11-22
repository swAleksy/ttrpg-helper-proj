namespace TtrpgHelperBackend.DTOs;

public class DashboardDto
{
    public string UserName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public string Email { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    
    public List<DashboardCharacterSummaryDto> Characters { get; set; } = new();
    // public List<DashboardSessionSummaryDto> Sessions { get; set; } = new();

    // todo - może kiedyś będzie
    // public List<DashboardMessageDto> Messages { get; set; } = new();
    // public DashboardQuickLinksDto QuickLinks { get; set; } = new();
}

public class DashboardCharacterSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RaceName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public int Level { get; set; }
}

// todo - Zaimplementować sejse
// public class DashboardSessionSummaryDto
// {
//
// }